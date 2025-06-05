using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Services.Devices;
using Sound2Light.Services.System;
using Sound2Light.Models.Audio;

using System.Diagnostics;

namespace Sound2Light.Startup
{
    public class SystemBootstrapper : ISystemBootstrapper
    {
        private readonly IAppConfigurationService _configService;
        private readonly IAsioDetectionService _asioDetectionService;
        private readonly IServiceProvider _services;

        public SystemBootstrapper(
            IAppConfigurationService configService,
            IAsioDetectionService asioDetectionService,
            IServiceProvider services)
        {
            _configService = configService;
            _asioDetectionService = asioDetectionService;
            _services = services;
        }

        public void Run()
        {
            // ASIO-Verfügbarkeit prüfen und global setzen
            var asioAvailable = _asioDetectionService.IsAsioAvailable();
            _configService.Settings.AsioAvailable = asioAvailable;
            Debug.WriteLine($"[System] ASIO verfügbar: {asioAvailable}");

            if (asioAvailable)
            {
                var asioDrivers = _services
                    .GetRequiredService<IAsioDriverDiscovery>()
                    .GetAsioDriverReferences();

                foreach (var driver in asioDrivers)
                {
                    Debug.WriteLine($"[ASIO] {driver.Name} → CLSID: {driver.Clsid}, DLL: {driver.DllPath}");

                    var device = AsioDriverProbe.TryCreateDevice(driver);
                    if (device == null)
                    {
                        Debug.WriteLine($"[ASIO] {driver.Name} → nicht verfügbar / konnte nicht initialisiert werden");
                        continue;
                    }

                    Debug.WriteLine($"[ASIO] {device.Name} - verfügbar: {device.IsAvailable}");
                    _configService.Settings.AvailableDevices.Add(device);
                }
            }

            // WASAPI-Geräte entdecken
            var wasapiDevices = _services
                .GetRequiredService<IWasapiDeviceDiscovery>()
                .GetWasapiDevices();

            foreach (var device in wasapiDevices)
            {
                _configService.Settings.AvailableDevices.Add(device);
                Debug.WriteLine($"[WASAPI] {device.Name} - {device.SampleRate} Hz, {device.BitDepth} Bit, {device.ChannelCount} Ch");
            }

            // TODO (folgt in nächstem Schritt):
            // - Validierung von appsettings.json (PreferredDevice)
            // - Wenn gültig: als CurrentDevice setzen, CaptureService starten
            // - Wenn ungültig oder fehlt: SetupCaptureWindow anzeigen
        }
    }
}
