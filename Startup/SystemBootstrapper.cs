using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Models.Audio;
using Sound2Light.Services.Audio;
using Sound2Light.Services.Devices;
using Sound2Light.ViewModels.Main;
using Sound2Light.ViewModels.Units;
using Sound2Light.ViewModels.Windows;

using System.Diagnostics;
using System.Linq;

namespace Sound2Light.Startup
{
    public class SystemBootstrapper : ISystemBootstrapper
    {
        private readonly IAppConfigurationService _configService;
        private readonly IServiceProvider _services;

        public SystemBootstrapper(
            IAppConfigurationService configService,
            IServiceProvider services)
        {
            _configService = configService;
            _services = services;
        }

        public void Run()
        {
            // Devices listen...
            var wasapiDevices = _services
                .GetRequiredService<IWasapiDeviceDiscovery>()
                .GetWasapiDevices();

            foreach (var device in wasapiDevices)
            {
                _configService.Settings.AvailableDevices.Add(device);
                Debug.WriteLine($"[WASAPI] {device.Name} - {device.SampleRate} Hz, {device.BitDepth} Bit, {device.ChannelCount} Ch");
            }

            var asioDevices = _services.GetRequiredService<IAsioDeviceEnumerator>().GetAvailableAsioDevices();
            foreach (var device in asioDevices)
            {
                _configService.Settings.AvailableDevices.Add(device);
                Debug.WriteLine($"[ASIO] {device.Name} - {device.SampleRate} Hz, {device.ChannelCount} Ch, SampleType: {device.SampleType}");
            }

            // PreferredDevice prüfen und CurrentDevice setzen
            var preferred = _configService.Settings.PreferredCaptureDevice;

            var allDevices = _configService.Settings.AvailableDevices;
            var current = preferred != null
                ? allDevices.FirstOrDefault(d =>
                    d.Type == preferred.Type &&
                    d.Name == preferred.Name &&
                    d.SampleRate == preferred.SampleRate &&
                    d.BitDepth == preferred.BitDepth &&
                    d.ChannelCount == preferred.ChannelCount)
                : null;

            _configService.Settings.CurrentDevice = current;

            // Hauptfenster öffnen (IMMER!)
            var mainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>()
            };
            mainWindow.Show();

            if (current != null && current.Type == AudioDeviceType.Asio)
            {
                var asioService = _services.GetRequiredService<AsioBridgeCLI.AsioCaptureService>();
                var bufferMultiplier = _configService.Settings.RingBufferSettings.LatencyMultiplier;

                bool started = asioService.Start(current.Name, bufferMultiplier);
                Debug.WriteLine($"[ASIO] CaptureService gestartet: {started}");

                if (started)
                {
                    asioService.RegisterConsumer("AsioVuMeter");
                    Debug.WriteLine("[ASIO] VuMeter-Consumer AsioVUMeter erfolgreich registriert.");
                }

                var captureViewModel = _services.GetRequiredService<UnitCaptureViewModel>();
                captureViewModel.SetCaptureSource(current.Type);
            }

            if (current != null && current.Type == AudioDeviceType.Wasapi)
            {
                var deviceBufferSize = current.BufferSize ?? 512;
                var ringBuffer = _services.GetRequiredService<WasapiRingBuffer>();
                ringBuffer.RegisterConsumer("WasapiVuMeter");

                var wasapiService = _services.GetRequiredService<WasapiCaptureService>();
                wasapiService.SamplesAvailable += buffer =>
                {
                    //Debug.WriteLine($"[SystemBootstrapper] SamplesAvailable-Ereignis! {buffer.Length} samples");
                    int frames = buffer.Length / 2;
                    ringBuffer.Write(buffer, 0, frames);
                };
                wasapiService.Start();

                Debug.WriteLine($"[WASAPI] VuMeter-Consumer WasapiVuMeter erfolgreich registriert.");

                // Das fehlte bisher!
                var captureViewModel = _services.GetRequiredService<UnitCaptureViewModel>();
                captureViewModel.SetCaptureSource(current.Type);
            }

            // Hinweis anzeigen, wenn KEIN gültiges Device da ist (aber App weiter starten!)
            if (current == null)
            {
                System.Windows.MessageBox.Show(
                    "Kein gültiges Audio-Gerät gefunden!\nBitte wählen Sie über das Menü 'Setup' ein Capture-Device aus.",
                    "Audio-Setup erforderlich",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
