using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Helpers.UI;
using Sound2Light.Views.Windows;
using Sound2Light.Services.Devices;

namespace Sound2Light.ViewModels.Units
{
    public class UnitSetupViewModel
    {
        private readonly IServiceProvider _services;

        public ICommand OpenCaptureSetupCommand { get; }
        public ICommand OpensACNSetupCommand { get; }
        public ICommand OpenartNETSetupCommand { get; }
        public ICommand OpenDMXSetupCommand { get; }
        public ICommand OpenS2LSetupCommand { get; }
        public ICommand OpenB2LSetupCommand { get; }
        public ICommand OpenMidiSetupCommand { get; }

        public UnitSetupViewModel(IServiceProvider services)
        {
            _services = services;

            OpenCaptureSetupCommand = new RelayCommand(_ => OpenCaptureSetupWindow());
            OpensACNSetupCommand = new RelayCommand(_ => OpensACNSetupWindow());
            OpenartNETSetupCommand = new RelayCommand(_ => OpenartNETSetupWindow());
            OpenDMXSetupCommand = new RelayCommand(_ => OpenDMXSetupWindow());
            OpenS2LSetupCommand = new RelayCommand(_ => OpenS2LSetupWindow());
            OpenB2LSetupCommand = new RelayCommand(_ => OpenB2LSetupWindow());
            OpenMidiSetupCommand = new RelayCommand(_ => OpenMidiSetupWindow());
        }

        private void OpenCaptureSetupWindow()
        {
            var appConfig = _services.GetRequiredService<IAppConfigurationService>();
            var window = new SetupCaptureWindow();

            var asioEnum = _services.GetRequiredService<IAsioDriverEnumerator>();
            var wasapiDisc = _services.GetRequiredService<IWasapiDeviceDiscovery>();

            var viewModel = new SetupCaptureViewModel(appConfig, asioEnum, wasapiDisc, () => window.Close());
            window.DataContext = viewModel;

            window.ShowDialog();
        }

        private void OpensACNSetupWindow()
        {
            var window = new SetupsACNWindow();
            window.ShowDialog();
        }

        private void OpenartNETSetupWindow()
        {
            var window = new SetupartNETWindow();
            window.ShowDialog();
        }

        private void OpenDMXSetupWindow()
        {
            var window = new SetupDMXWindow();
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
