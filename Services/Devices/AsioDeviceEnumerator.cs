using System.Collections.Generic;
using System.Linq;
using Sound2Light.Models.Audio;
using Sound2Light.Contracts.Services.Devices;
using AsioBridgeCLI;

namespace Sound2Light.Services.Devices
{
    public class AsioDeviceEnumerator : IAsioDeviceEnumerator
    {
        public List<AudioDevice> GetAvailableAsioDevices()
        {
            var asioDevices = AsioBridge.ListAvailableDevices();

            return asioDevices.Select(d => new AudioDevice
            {
                Name = d.Name,
                Type = AudioDeviceType.Asio,
                SampleRate = (int)d.SampleRate,
                BitDepth = 32, // oder aus SampleType ableiten
                ChannelCount = d.InputChannels,
                SampleType = d.SampleType,
                BufferSize = d.BufferSize,
            }).ToList();
        }
    }
}
