using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound2Light.Common
{
    public class AudioDataEventArgs : EventArgs
    {
        public float[] Buffer { get; set; } = [];
        public Models.AudioDeviceInfo DeviceInfo { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}