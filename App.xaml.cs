using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Audio;
using Sound2Light.Contracts.Services.Audio;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Services.Audio;
using Sound2Light.Services.Devices;
using Sound2Light.Services.System;
using Sound2Light.Services.UI;
using Sound2Light.Startup;
using Sound2Light.ViewModels.Main;
using Sound2Light.ViewModels.Units;
using Sound2Light.ViewModels.Windows;

using System.IO;
using System.Windows;

namespace Sound2Light
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;
        private ISystemBootstrapper _bootstrapper = null!;
        public IServiceProvider Services => _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();

            // Konfigurations-Service
            serviceCollection.AddSingleton<IAppConfigurationService>(provider =>
            {
                var configService = new AppConfigurationService();
                configService.LoadConfiguration();
                return configService;
            });

            // AppSettings direkt bereitstellen
            serviceCollection.AddSingleton(provider =>
                provider.GetRequiredService<IAppConfigurationService>().Settings);

            // UI-Dienste
            serviceCollection.AddSingleton<IApplicationShutdownService, ApplicationShutdownService>();
            serviceCollection.AddSingleton<IPowerButtonStateService, PowerButtonStateService>();

            // Device Services
            serviceCollection.AddSingleton<IWasapiDeviceDiscovery, WasapiDeviceDiscovery>();
            serviceCollection.AddSingleton<IAsioDeviceEnumerator, AsioDeviceEnumerator>();
            // AsioBridge (C++/CLI) Capture Service
            serviceCollection.AddSingleton<AsioBridgeCLI.AsioCaptureService>();
            // Wasapi Capture Service
            serviceCollection.AddSingleton<WasapiCaptureService>(provider =>
            {
                var config = provider.GetRequiredService<IAppConfigurationService>();
                var current = config.Settings.CurrentDevice!;
                var deviceBufferSize = current.BufferSize ?? 512;
                var ringBuffer = provider.GetRequiredService<WasapiRingBuffer>();
                return new WasapiCaptureService(current, deviceBufferSize, ringBuffer);
            });

            // WasapiRingBuffer – jetzt korrekt per Factory, damit RingBufferSize gesetzt wird!
            serviceCollection.AddSingleton<WasapiRingBuffer>(provider =>
            {
                var config = provider.GetRequiredService<IAppConfigurationService>();
                var ringBufferSize = config.Settings.RingBufferSettings.RingBufferSize;
                return new WasapiRingBuffer(ringBufferSize); // ***KEIN Multiplizieren mit 2!***
            });
            serviceCollection.AddSingleton<IWasapiRingBuffer>(provider =>
                provider.GetRequiredService<WasapiRingBuffer>());

            // Audio Analysis Buffer
            serviceCollection.AddSingleton<MonoAnalysisBuffer>();
            serviceCollection.AddSingleton<MonoFeederService>();

            // ViewModels
            serviceCollection.AddSingleton<UnitSetupViewModel>();
            serviceCollection.AddSingleton<UnitCaptureViewModel>();
            serviceCollection.AddSingleton<PowerButtonViewModel>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddTransient<SetupCaptureViewModel>();
            serviceCollection.AddTransient<SetupDMXViewModel>();

            // Bootstrapper
            serviceCollection.AddSingleton<ISystemBootstrapper, SystemBootstrapper>();

            // Build Provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Starte Initialisierung
            _bootstrapper = _serviceProvider.GetRequiredService<ISystemBootstrapper>();
            _bootstrapper.Run();
        }
    }
}
