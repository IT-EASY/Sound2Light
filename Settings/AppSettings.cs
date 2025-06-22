using Sound2Light.Models.Audio;
using Sound2Light.Models.DMX512;

using System.Collections.ObjectModel;

namespace Sound2Light.Settings;

public class AppSettings
{
    public AudioDevice? PreferredCaptureDevice { get; set; }

    public AudioDevice? CurrentDevice { get; set; }

    public RingBufferSettings RingBufferSettings { get; set; } = new();

    public DmxMappingConfig DmxMapping { get; set; } = new DmxMappingConfig();

    public ObservableCollection<AudioDevice> AvailableDevices { get; set; } = new();

}
