namespace Sound2Light.Models
{
    public class AudioDeviceInfo
    {
        // Device Identification
        public AudioApiType ApiType { get; set; }
        public string TechnicalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        // Hardware-spezifische Eigenschaften
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public int ChannelOffset { get; set; }
        public int BitDepth { get; set; } = 16; // Standard: 16 Bit
        public List<int> SupportedSampleRates { get; set; } = [];
        public List<int> SupportedBufferSizes { get; set; } = [];

        // Aktive Konfiguration (wird bei Initialisierung gesetzt)
        public int CurrentSampleRate { get; set; }
        public int CurrentBufferSize { get; set; }
    }
    public enum AudioApiType
    {
        ASIO,
        WASAPI
    }
}
