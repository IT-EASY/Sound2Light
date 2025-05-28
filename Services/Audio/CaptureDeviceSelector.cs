// Datei: Services/Audio/CaptureDeviceSelector.cs
using Sound2Light.Models.Audio;
using Sound2Light.Settings;
using System.Linq;

namespace Sound2Light.Services.Audio
{
    public static class CaptureDeviceSelector
    {
        public static DeviceInfo? SelectValidDevice(AppSettings config)
        {
            var preferred = config.PreferredCaptureDevice;

            var asioDevices = AudioDeviceDiscovery.GetAsioDevices(config); 
            
            if (asioDevices.Any())
            {
                if (preferred != null && preferred.Type == AudioDeviceType.ASIO) // ASIO preferred
                {
                    var match = asioDevices.FirstOrDefault(d => d.Name == preferred.Name);
                    if (match != null)
                        return match;
                }

                // Kein ASIO preferred oder nicht verfügbar → erstes ASIO nehmen
                return asioDevices.First();
            }

            // ASIO nicht verfügbar → WASAPI prüfen
            var wasapiDevices = AudioDeviceDiscovery.GetWasapiDevices();
            if (wasapiDevices.Any())
            {
                if (preferred != null && preferred.Type == AudioDeviceType.WASAPI) // WASAPI preferred
                {
                    var match = wasapiDevices.FirstOrDefault(d => d.DisplayName == preferred.Name);
                    if (match != null)
                        return match;
                }

                return wasapiDevices.First();
            }

            // Nichts verfügbar
            return null;
        }
    }
}
