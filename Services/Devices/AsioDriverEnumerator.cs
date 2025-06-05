using System.Collections.Generic;
using Sound2Light.Models.Audio;
using Sound2Light.Contracts.Services.Devices;

namespace Sound2Light.Services.Devices
{
    public class AsioDriverEnumerator : IAsioDriverEnumerator
    {
        private readonly AsioDriverDiscovery _discovery;

        public AsioDriverEnumerator(AsioDriverDiscovery discovery)
        {
            _discovery = discovery;
        }

        public List<AudioDevice> GetAvailableAsioDevices()
        {
            var result = new List<AudioDevice>();

            foreach (var reference in _discovery.GetAsioDriverReferences())
            {
                var device = AsioDriverProbe.TryCreateDevice(reference);
                if (device != null)
                {
                    result.Add(device);
                }
            }

            return result;
        }
    }
}
