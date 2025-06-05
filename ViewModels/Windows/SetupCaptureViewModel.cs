using Sound2Light.Config;
using Sound2Light.Models.Audio;
using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Helpers.UI;
using Sound2Light.Services.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class SetupCaptureViewModel : INotifyPropertyChanged
{
    private readonly IAppConfigurationService _config;
    private readonly IAsioDriverEnumerator _asioEnumerator;
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

    public string DummyDeviceName => "[Kein Gerät verfügbar – Setup vorbereitet]";

    public int SelectedFFTSize { get; set; } = 1024;
    public int SelectedBufferMultiplier { get; set; } = 2;

    public ICommand ApplyTemporaryCommand { get; }
    public ICommand SaveAndCloseCommand { get; }

    public SetupCaptureViewModel(
        IAppConfigurationService config,
        IAsioDriverEnumerator asioEnumerator,
        IWasapiDeviceDiscovery wasapiDiscovery,
        Action closeCallback)
    {
        _config = config;
        _asioEnumerator = asioEnumerator;
        _wasapiDiscovery = wasapiDiscovery;
        _closeCallback = closeCallback;

        ApplyTemporaryCommand = new RelayCommand(_ => ApplyTemporaryDevice());
        SaveAndCloseCommand = new RelayCommand(_ => SaveAndClose());

        LoadDevices();
    }

    private void LoadDevices()
    {
        AvailableDevices.Clear();

        var asio = _asioEnumerator.GetAvailableAsioDevices();
        var wasapi = _wasapiDiscovery.GetWasapiDevices();

        foreach (var device in asio)
            AvailableDevices.Add(new DisplayDevice(device, "[ASIO]"));

        foreach (var device in wasapi)
            AvailableDevices.Add(new DisplayDevice(device, "[WASAPI]"));

        SelectedDevice = AvailableDevices.FirstOrDefault();
    }

    private void ApplyTemporaryDevice()
    {
        // später: temporär aktivieren
    }

    private void SaveAndClose()
    {
        ApplyTemporaryDevice();
        _closeCallback.Invoke();
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
}
