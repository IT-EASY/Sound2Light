using System.Collections.Generic;

using Sound2Light.Models.Audio;

namespace Sound2Light.Contracts.Services.Devices
{
    public interface IAsioDeviceEnumerator
    {
        List<AudioDevice> GetAvailableAsioDevices();
    }
}