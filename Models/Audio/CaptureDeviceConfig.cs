using Sound2Light.Models.Audio;

namespace Sound2Light.Models.Audio
{
    /// <summary>
    /// Struktur zur Beschreibung eines Audio-Capture-Geräts (für Konfiguration & Laufzeit)
    /// </summary>
    public class CaptureDeviceConfig
    {
        public string Name { get; set; } = string.Empty;
        public AudioDeviceType Type { get; set; }

        public int SampleRate { get; set; }
        public int BitDepth { get; set; }
        public int Channels { get; set; }
        public int BufferSize { get; set; }
    }
}
