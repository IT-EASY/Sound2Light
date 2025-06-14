using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Models.Audio;
using Sound2Light.Services.Audio;
using Sound2Light.Services.Devices;
using Sound2Light.ViewModels.Main;
using Sound2Light.ViewModels.Windows;

using System.Diagnostics;

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
            }

            if (current != null && current.Type == AudioDeviceType.Wasapi)
            {
                // Echte Gerätebuffergröße pro Block (Frames)
                var deviceBufferSize = current.BufferSize ?? 512;

                // *** Hol DIR IMMER DIE DI-Instanz, NIE new! ***
                var ringBuffer = _services.GetRequiredService<WasapiRingBuffer>();

                // Service bekommt die Device-Buffergröße (Frames), nicht RingBufferSize!
                var wasapiService = new WasapiCaptureService(current, deviceBufferSize);

                wasapiService.SamplesAvailable += buffer =>
                {
                    int frames = buffer.Length / 2; // Stereo: 2 Samples pro Frame
                    ringBuffer.Write(buffer, 0, frames);
                };

                wasapiService.Start();
                Debug.WriteLine($"[WASAPI] CaptureService gestartet für: {current.Name}, DeviceBufferSize: {deviceBufferSize}, RingBufferSize: {ringBuffer.Capacity}, Multiplier: {_configService.Settings.RingBufferSettings.LatencyMultiplier}");
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
