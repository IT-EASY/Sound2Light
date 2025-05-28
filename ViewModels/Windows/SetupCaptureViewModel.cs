using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

using Sound2Light.Models.Audio;
using Sound2Light.Services.Audio;
using Sound2Light.Helpers.UI;
using Sound2Light.Config;
using Sound2Light.Settings;

namespace Sound2Light.ViewModels.Windows
{
    public class SetupCaptureViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DeviceInfo> AvailableDevices { get; } = new();

        private DeviceInfo? _selectedDevice;
        public DeviceInfo? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PreferredSampleRate));
                    OnPropertyChanged(nameof(PreferredBitDepth));
                    OnPropertyChanged(nameof(PreferredInputChannels));
                    OnPropertyChanged(nameof(PreferredBufferSize));
                }
            }
        }

        public int? PreferredSampleRate => SelectedDevice?.PreferredSampleRate;
        public int? PreferredBitDepth => SelectedDevice?.PreferredBitDepth;
        public int? PreferredInputChannels => SelectedDevice?.PreferredInputChannels;
        public int? PreferredBufferSize => SelectedDevice?.PreferredBufferSize;

        public List<int> FFTSizes { get; } = new() { 256, 512, 1024, 2048, 4096 };
        public List<int> BufferMultipliers { get; } = new() { 1, 2, 3, 4, 5 };

        private int _selectedFFTSize;
        public int SelectedFFTSize
        {
            get => _selectedFFTSize;
            set => SetProperty(ref _selectedFFTSize, value);
        }

        private int _selectedBufferMultiplier;
        public int SelectedBufferMultiplier
        {
            get => _selectedBufferMultiplier;
            set => SetProperty(ref _selectedBufferMultiplier, value);
        }

        public ICommand CancelCommand { get; }
        public ICommand SaveSessionOnlyCommand { get; }
        public ICommand SaveAndCloseCommand { get; }

        private readonly Action _closeAction;
        private readonly IAppConfigurationService _configService;

        public SetupCaptureViewModel(IAppConfigurationService configService, Action closeAction)
        {
            _configService = configService;
            _closeAction = closeAction;

            CancelCommand = new RelayCommand(_ => _closeAction());
            SaveSessionOnlyCommand = new RelayCommand(_ => ApplyTemporaryDevice());
            SaveAndCloseCommand = new RelayCommand(_ => SaveAndClose());

            var devices = AudioDeviceDiscovery
                .GetAsioDevices(_configService.Settings)
                .Concat(AudioDeviceDiscovery.GetWasapiDevices());

            foreach (var device in devices)
                AvailableDevices.Add(device);

            if (AvailableDevices.Any())
                SelectedDevice = AvailableDevices.First();

            // 🧠 FFT + Multiplier aus gespeicherter Konfiguration übernehmen
            SelectedFFTSize = _configService.Settings.RingBuffer.FFTSize;
            SelectedBufferMultiplier = _configService.Settings.RingBuffer.BufferMultiplier;
        }

        private void ApplyTemporaryDevice()
        {
            if (SelectedDevice == null)
                return;

            _configService.Settings.PreferredCaptureDevice = new CaptureDeviceConfig
            {
                Name = SelectedDevice.Name,
                Type = SelectedDevice.Type,
                SampleRate = SelectedDevice.PreferredSampleRate ?? 0,
                BitDepth = SelectedDevice.PreferredBitDepth ?? 0,
                Channels = SelectedDevice.PreferredInputChannels ?? 0,
                BufferSize = SelectedDevice.PreferredBufferSize ?? 0
            };

            _configService.Settings.RingBuffer.FFTSize = SelectedFFTSize;
            _configService.Settings.RingBuffer.BufferMultiplier = SelectedBufferMultiplier;

            _closeAction();
        }

        private void SaveAndClose()
        {
            ApplyTemporaryDevice();
            _configService.SaveConfiguration();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
