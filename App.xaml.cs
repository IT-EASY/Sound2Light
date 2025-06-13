using Microsoft.Extensions.DependencyInjection;
using Sound2Light.Startup;
using Sound2Light.ViewModels.Main;
using Sound2Light.ViewModels.Units;
using Sound2Light.ViewModels.Windows;
using Sound2Light.Services.UI;
using Sound2Light.Services.System;
using Sound2Light.Config;
using System.IO;
using System.Windows;
using Sound2Light.Services.Devices;
using Sound2Light.Contracts.Services.Devices;

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

            // Konfigurationspfad
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configDir = Path.Combine(appDataPath, "Sound2Light");
            var configFilePath = Path.Combine(configDir, "appsettings.json");

            // Konfigurations-Service
            serviceCollection.AddSingleton<IAppConfigurationService>(provider =>
            {
                var configService = new AppConfigurationService(configFilePath);
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


            // ViewModels
            serviceCollection.AddSingleton<UnitSetupViewModel>();
            serviceCollection.AddSingleton<UnitCaptureViewModel>();
            serviceCollection.AddSingleton<PowerButtonViewModel>();
            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddTransient<SetupCaptureViewModel>();

            // Bootstrapper
            serviceCollection.AddSingleton<ISystemBootstrapper, SystemBootstrapper>();

            // Build Provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Starte Initialisierung
            _bootstrapper = _serviceProvider.GetRequiredService<ISystemBootstrapper>();
            _bootstrapper.Run(); // aktuell leer



        }
    }
}
