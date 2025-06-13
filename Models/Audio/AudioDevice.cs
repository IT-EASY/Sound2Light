using System.Text.Json.Serialization;

namespace Sound2Light.Models.Audio
{
    public class AudioDevice
    {
        public string Name { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AudioDeviceType Type { get; set; }

        public int SampleRate { get; set; }
        public int BitDepth { get; set; }
        public int ChannelCount { get; set; }
        public int? BufferSize { get; set; } // optional: nur ASIO

        public string[]? InputChannelNames { get; set; }
        public int? SampleType { get; set; } // Nur ASIO relevant

        [JsonIgnore]
        public string DisplayName => $"[{Type}] {Name}"; // Nur für DeviceList in der UI!
    }
}
