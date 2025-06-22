using System.Diagnostics;

using Sound2Light.Settings;
using Sound2Light.Services.RegistryAccess;

namespace Sound2Light.Config
{
    public class AppConfigurationService : IAppConfigurationService
    {

        public AppSettings Settings { get; private set; }

        public AppConfigurationService() 
        {
            Settings = new AppSettings();
        }

        public void LoadConfiguration()
        {
            // Registry-Lesevorgänge
            Settings.CurrentDevice = CaptureDeviceRegistryLoader.LoadCurrentDevice();
            Settings.PreferredCaptureDevice = CaptureDeviceRegistryLoader.LoadPreferredDevice();
            Settings.RingBufferSettings = RingBufferRegistryLoader.LoadWasapiSettings();

            Debug.WriteLine("[AppSettings] Konfiguration erfolgreich aus Registry geladen.");
        }

        public void SaveConfiguration()
        {
            if (Settings.CurrentDevice != null)
                CaptureDeviceRegistryWriter.SaveCurrentDevice(Settings.CurrentDevice);

            if (Settings.PreferredCaptureDevice != null)
                CaptureDeviceRegistryWriter.SavePreferredDevice(Settings.PreferredCaptureDevice);

            RingBufferRegistryWriter.SaveWasapiSettings(Settings.RingBufferSettings);
        }
    }
}
