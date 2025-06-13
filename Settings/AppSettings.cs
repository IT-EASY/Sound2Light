using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

using Sound2Light.Models.Audio;

namespace Sound2Light.Settings;

public class AppSettings
{
    /// <summary>
    /// Bevorzugtes Aufnahmegerät (wird gespeichert).
    /// </summary>
    public AudioDevice? PreferredCaptureDevice { get; set; }

    /// <summary>
    /// Aktuell verwendetes Gerät zur Laufzeit.
    /// </summary>
    [JsonIgnore]
    public AudioDevice? CurrentDevice { get; set; }

    /// <summary>
    /// Verfügbare Geräte zur Laufzeit (ASIO/WASAPI).
    /// </summary>
    [JsonIgnore]
    public ObservableCollection<AudioDevice> AvailableDevices { get; set; } = new();


    /// <summary>
    /// Einstellungen für den internen Audio-Ringpuffer.
    /// </summary>
    public RingBufferSettings RingBufferSettings { get; set; } = new();
}
