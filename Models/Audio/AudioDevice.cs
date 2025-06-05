using System.Text.Json.Serialization;

namespace Sound2Light.Models.Audio;

public class AudioDevice
{
    public string Name { get; set; } = string.Empty;
    public string? OriginalDisplayName { get; set; }
    public AudioDeviceType Type { get; set; }

    public int SampleRate { get; set; }
    public int BitDepth { get; set; }
    public int ChannelCount { get; set; }
    public int? BufferSize { get; set; } // optional: nur ASIO

    public string[]? InputChannelNames { get; set; }

    [JsonIgnore]
    public bool IsAvailable { get; set; } = true;

    [JsonIgnore]
    public string DisplayName => $"[{Type}] {Name}";
}
