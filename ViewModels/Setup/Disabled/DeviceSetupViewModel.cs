// Sound2Light/ViewModels/Setup/DeviceSetupViewModel.cs  
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using Sound2Light.Services.Audio;

namespace Sound2Light.ViewModels.Setup.Disabled
{
    public class DeviceSetupViewModel : INotifyPropertyChanged
    {
        // Properties  
        public ObservableCollection<string> AsioDrivers { get; } = new();
        public ObservableCollection<string> WasapiDevices { get; } = new();
        public string? SelectedAsioDriver { get; set; }
        public string? SelectedWasapiDevice { get; set; }

        // Commands  
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Dependency Injection  
        private readonly IAudioDeviceManager _deviceManager;

        public DeviceSetupViewModel(IAudioDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
            LoadDevices();
            InitializeCommands();
        }

        private void LoadDevices()
        {
            // ASIO/WASAPI-Devices laden (später implementieren)  
            foreach (var driver in _deviceManager.GetAsioDrivers())
                AsioDrivers.Add(driver);

            foreach (var device in _deviceManager.GetWasapiDevices())
                WasapiDevices.Add(device);
        }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(SaveConfiguration);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveConfiguration()
        {
            // TODO: Speichern in AppSettings  
        }

        private void Cancel()
        {
            // TODO: Fenster schließen  
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}