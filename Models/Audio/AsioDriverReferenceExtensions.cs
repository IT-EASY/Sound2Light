using Sound2Light.Models.Audio;

using System.IO;

namespace Sound2Light.Models.Audio
{
    public static class AsioDriverReferenceExtensions
    {
        public static AudioDevice ToAudioDevice(this AsioDriverReference reference)
        {
            return new AudioDevice
            {
                Name = reference.Name,
                Type = AudioDeviceType.Asio,
                IsAvailable = File.Exists(reference.DllPath),
                SampleRate = 0,
                BitDepth = 0,
                ChannelCount = 0,
                BufferSize = null,
                InputChannelNames = null,
                OriginalDisplayName = reference.Name
            };
        }
    }
}
