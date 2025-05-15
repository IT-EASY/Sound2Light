using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound2Light.Models
{
    public class AudioSettings
    {
        public int ChannelOffset { get; set; } = 1;         // Standard: 0 (Mono) oder 1 (Stereo)
        public int BufferSize { get; set; } = 1024;         // Standard: 1024 Samples
        public int SampleRate { get; set; } = 48000;        // Standard: 48 kHz
        public int Channels { get; set; } = 2;              // Standard: Stereo
        public int RingBufferMultiplier { get; set; } = 4;  // Standard: 4x BufferSize
    }
}
