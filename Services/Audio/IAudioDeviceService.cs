using Sound2Light.Models.Audio;

using System.Collections.Generic;

namespace Sound2Light.Services.Audio
{
    public interface IAudioDeviceService
    {
        List<DeviceInfo> GetAsioDevices();
        List<DeviceInfo> GetWasapiDevices();

        /// <summary>
        /// Gibt eine kombinierte, gefilterte Liste aller verfügbaren Geräte zurück
        /// </summary>
        List<DeviceInfo> GetAvailableCaptureDevices();
    }
}
