using Sound2Light.Models.Audio;
using Sound2Light.Settings;
using Sound2Light.Config;

namespace Sound2Light.Services.Audio
{
    public class AudioDeviceService : IAudioDeviceService
    {
        private readonly IAppConfigurationService _configService;

        public AudioDeviceService(IAppConfigurationService configService)
        {
            _configService = configService;
        }

        public List<DeviceInfo> GetAsioDevices()
        {
            return AudioDeviceDiscovery.GetAsioDevices(_configService.Settings);
        }

        public List<DeviceInfo> GetWasapiDevices()
        {
            return AudioDeviceDiscovery.GetWasapiDevices();
        }

        public List<DeviceInfo> GetAvailableCaptureDevices()
        {
            var asio = GetAsioDevices();
            var wasapi = GetWasapiDevices();
            return asio.Concat(wasapi).ToList();
        }
    }
}
