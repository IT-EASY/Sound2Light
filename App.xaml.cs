// Sound2Light/App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Sound2Light.Startup;
using Sound2Light.ViewModels.Main;
using Sound2Light.ViewModels.Units;
using Sound2Light.Services.UI;
using Sound2Light.Services.System;
using Sound2Light.Config;
using Sound2Light.Settings;

using System;
using System.IO;
using System.Windows;
using Sound2Light.ViewModels.Windows;
using Sound2Light.Services.Audio;

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

            // 🔧 Schritt 1: DI konfigurieren und ServiceProvider erstellen
            var serviceCollection = new ServiceCollection();

            // 🧩 Konfigurationspfad festlegen (AppData → Sound2Light\appsettings.json)
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var configDir = Path.Combine(appDataPath, "Sound2Light");
            var configFilePath = Path.Combine(configDir, "appsettings.json");

            // 🧩 Konfigurations-Service als Singleton registrieren
            serviceCollection.AddSingleton<IAppConfigurationService>(provider =>
            {
                var configService = new AppConfigurationService(configFilePath);
                configService.LoadConfiguration(); // direkt beim Start laden
                return configService;
            });

            // 🧩 Services & Logik
            serviceCollection.AddSingleton<IApplicationShutdownService, ApplicationShutdownService>();
            serviceCollection.AddSingleton<IPowerButtonStateService, PowerButtonStateService>();
            // 🧩 Geräte-Erkennung (ASIO/WASAPI)
            serviceCollection.AddSingleton<IAudioDeviceService, AudioDeviceService>();


            // 🧩 ViewModels (richtige Reihenfolge beachten)
            // 🧩 ViewModel für UnitSetup
            serviceCollection.AddSingleton<UnitSetupViewModel>();
            // 🧩 ViewModel für SetupCaptureWindow
            serviceCollection.AddTransient<SetupCaptureViewModel>();
            // 🧩 ViewModel für PowerButton
            serviceCollection.AddSingleton<PowerButtonViewModel>();
            // 🧩 ViewModel für Main
            serviceCollection.AddSingleton<MainViewModel>();

            // 🧩 Registrierung des Bootstrappers
            serviceCollection.AddSingleton<ISystemBootstrapper, SystemBootstrapper>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            // 🚀 Schritt 2: Startroutine aufrufen (koordiniert Initialisierung)
            _bootstrapper = _serviceProvider.GetRequiredService<ISystemBootstrapper>();
            _bootstrapper.Run();

            // 🧷 Schritt 3: MainWindow starten und mit ViewModel verbinden
            var mainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainViewModel>()
            };
            mainWindow.Show();
        }
    }
}
