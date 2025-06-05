using Sound2Light.Models.Audio;

using System.Collections.Generic;

namespace Sound2Light.Contracts.Services.Devices
{
    public interface IAsioDriverDiscovery
    {
        List<AsioDriverReference> GetAsioDriverReferences();
    }
}
