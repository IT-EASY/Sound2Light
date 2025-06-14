using Sound2Light.Config;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Helpers.UI;
using Sound2Light.Helpers.Audio;
using Sound2Light.Models.Audio;
using Sound2Light.Services.Devices;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sound2Light.ViewModels.Windows
{
    public class SetupCaptureViewModel : INotifyPropertyChanged
    {
        private readonly IAppConfigurationService _config;
        private readonly IAsioDeviceEnumerator _asioEnumerator;
        private readonly IWasapiDeviceDiscovery _wasapiDiscovery;
        private readonly Action _closeCallback;

        public ObservableCollection<DisplayDevice> AvailableDevices { get; } = new();

        private DisplayDevice? _selectedDevice;
        public DisplayDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                }
            }
        }


        // BufferSize für WASAPI (wenn relevant)
        public int[] WasapiBufferSizes { get; } = new[] { 192, 256, 384, 512, 1024, 2048 };
        private const int DefaultWasapiBufferSize = 512;
        private int _selectedWasapiBufferSize = DefaultWasapiBufferSize;
        public int SelectedWasapiBufferSize
        {
            get => _selectedWasapiBufferSize;
            set
            {
                if (_selectedWasapiBufferSize != value)
                {
                    _selectedWasapiBufferSize = value;
                    OnPropertyChanged();
                }
            }
        }

        // Anzeige für Preferred Device
        public string PreferredDeviceDisplay =>
            _config.Settings.PreferredCaptureDevice?.Name ?? "-";
        // Anzeige für Current Device
        public string CurrentDeviceDisplay =>
            _config.Settings.CurrentDevice != null
            ? _config.Settings.CurrentDevice.Name
            : "-";

        // Command für Button "Set Preferred"
        public ICommand SavePreferredDeviceCommand { get; }
        // Command für "Set Current"
        public ICommand SaveCurrentDeviceCommand { get; }
        // Command für "Close"
        public ICommand CloseCommand { get; }

        public SetupCaptureViewModel(
            IAppConfigurationService config,
            IAsioDeviceEnumerator asioEnumerator,
            IWasapiDeviceDiscovery wasapiDiscovery,
            Action closeCallback)
        {
            _config = config;
            _asioEnumerator = asioEnumerator;
            _wasapiDiscovery = wasapiDiscovery;
            _closeCallback = closeCallback;

            SavePreferredDeviceCommand = new RelayCommand(_ => SavePreferredDevice());
            SaveCurrentDeviceCommand = new RelayCommand(_ => SaveCurrentDevice());
            CloseCommand = new RelayCommand(_ => _closeCallback?.Invoke());

            FillDeviceListBox();
        }

        private void FillDeviceListBox()
        {
            AvailableDevices.Clear();


            var sortedDevices = _config.Settings.AvailableDevices
                .Where(d => d.Type == AudioDeviceType.Asio || d.Type == AudioDeviceType.Wasapi)
                .OrderBy(d => d.Type) // ASIO vor WASAPI (enum order)
                .ThenBy(d => d.Name)  // Innerhalb nach Name
                .ToList();

            foreach (var device in sortedDevices)
            {
                var prefix = device.Type == AudioDeviceType.Asio ? "[ASIO]" : "[WASAPI]";
                AvailableDevices.Add(new DisplayDevice(device, prefix));
            }

            // Preferred vorauswählen (wenn vorhanden)
            var preferred = _config.Settings.PreferredCaptureDevice;
            SelectedDevice = preferred != null
                ? AvailableDevices.FirstOrDefault(d => d.Device.Name == preferred.Name && d.Device.Type == preferred.Type)
                : AvailableDevices.FirstOrDefault();
        }

        // "Set Preferred Device" – Device speichern und Property aktualisieren
        private void SavePreferredDevice()
        {
            if (SelectedDevice?.Device != null)
            {
                _config.Settings.PreferredCaptureDevice = SelectedDevice.Device;
                _config.SaveConfiguration();
                OnPropertyChanged(nameof(PreferredDeviceDisplay));
            }
        }
        // "Set Current Device" – nur für die Laufzeit
        private void SaveCurrentDevice()
        {
            if (SelectedDevice?.Device != null)
            {
                _config.Settings.CurrentDevice = SelectedDevice.Device;
                OnPropertyChanged(nameof(CurrentDeviceDisplay));
                System.Windows.MessageBox.Show(
                    "Das Eingabegerät wurde geändert. Bitte starten Sie die Anwendung neu, damit die Änderung wirksam wird.",
                    "Neustart erforderlich",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class DisplayDevice
    {
        public string DisplayName { get; }
        public AudioDevice Device { get; }

        public DisplayDevice(AudioDevice device, string prefix)
        {
            Device = device;
            DisplayName = $"{prefix} {device.Name}";
        }

        public bool IsAsio => Device.Type == AudioDeviceType.Asio;
        public bool IsWasapi => Device.Type == AudioDeviceType.Wasapi;

        public string BufferSizeDisplay => Device.BufferSize?.ToString() ?? "-";

        public string SampleTypeDisplay => AsioSampleTypeHelper.GetDescription(Device.SampleType);
        public bool IsSampleTypeEnabled => IsAsio && Device.SampleType.HasValue;
    }
}
