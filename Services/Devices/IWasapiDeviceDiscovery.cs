using System.Collections.Generic;

using Sound2Light.Models.Audio;

namespace Sound2Light.Services.Devices
{
    public interface IWasapiDeviceDiscovery
    {
        List<AudioDevice> GetWasapiDevices();
    }
}
