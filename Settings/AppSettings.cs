using Sound2Light.Models.Audio;

namespace Sound2Light.Settings
{
    public class AppSettings
    {
        public RingBufferSettings RingBuffer { get; set; } = new();
        public CaptureDeviceConfig? PreferredCaptureDevice { get; set; }

        public List<string> IgnoredAsioDrivers { get; set; } = new()
        {
            "Steinberg built-in ASIO Driver",
            "ASIO4ALL",
            "FL Studio ASIO",
            "FlexASIO"
        };
        public class RingBufferSettings
        {
            public int FFTSize { get; set; } = 2048;             // Default: 2048 Samples
            public int BufferMultiplier { get; set; } = 4;       // Default: 4× FFTSize
        }
    }
}
