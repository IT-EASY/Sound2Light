using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Helpers.UI;
using Sound2Light.ViewModels.Windows;
using Sound2Light.ViewModels.Addon;
using Sound2Light.Views.Windows;
using Sound2Light.Services.Devices;

namespace Sound2Light.ViewModels.Units
{
    public class UnitSetupViewModel
    {
        private readonly IServiceProvider _services;

        public ICommand OpenCaptureSetupCommand { get; }
        public ICommand OpenDMXSetupCommand { get; }
        public ICommand OpenS2LSetupCommand { get; }
        public ICommand OpenB2LSetupCommand { get; }
        public ICommand OpenMidiSetupCommand { get; }

        public UnitSetupViewModel(IServiceProvider services)
        {
            _services = services;

            OpenCaptureSetupCommand = new RelayCommand(_ => OpenCaptureSetupWindow());
            OpenDMXSetupCommand = new RelayCommand(_ => OpenDMXSetupWindow());
            OpenS2LSetupCommand = new RelayCommand(_ => OpenS2LSetupWindow());
            OpenB2LSetupCommand = new RelayCommand(_ => OpenB2LSetupWindow());
            OpenMidiSetupCommand = new RelayCommand(_ => OpenMidiSetupWindow());
        }

        private void OpenCaptureSetupWindow()
        {
            var appConfig = _services.GetRequiredService<IAppConfigurationService>();
            var window = new SetupCaptureWindow();

            var asioEnum = _services.GetRequiredService<IAsioDeviceEnumerator>();
            var wasapiDisc = _services.GetRequiredService<IWasapiDeviceDiscovery>();

            var viewModel = new SetupCaptureViewModel(appConfig, asioEnum, wasapiDisc, () => window.Close());
            window.DataContext = viewModel;

            window.ShowDialog();
        }


        private void OpenDMXSetupWindow()
        {
            var appConfig = _services.GetRequiredService<IAppConfigurationService>();
            var window = new SetupDMXWindow();

            // Hier ViewModel MIT Fenster-Close-Callback erzeugen!
            var viewModel = new Sound2Light.ViewModels.Addon.SetupDMXViewModel(appConfig, () => window.Close());
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        private void OpenS2LSetupWindow()
        {
            var window = new SetupS2LWindow();
            window.ShowDialog();
        }

        private void OpenB2LSetupWindow()
        {
            var window = new SetupB2LWindow();
            window.ShowDialog();
        }

        private void OpenMidiSetupWindow()
        {
            var window = new SetupMidiWindow();
            window.ShowDialog();
        }
    }
}
