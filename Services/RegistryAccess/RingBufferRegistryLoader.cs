using Microsoft.Win32;

using Sound2Light.Models.Audio;
using Sound2Light.Settings;

using System.Diagnostics;

namespace Sound2Light.Services.RegistryAccess
{
    public static class RingBufferRegistryLoader
    {
        private const string WasapiPath = @"Software\Sound2Light\RingBuffers\WasapiDevice";

        public static RingBufferSettings LoadWasapiSettings()
        {
            var settings = new RingBufferSettings();

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(WasapiPath);
                if (key == null)
                {
                    Debug.WriteLine("[Registry] WasapiBuffer-Schlüssel nicht gefunden. Es werden Standardwerte verwendet.");
                    return settings;
                }

                settings.LatencyMultiplier = (int)(key.GetValue("LatencyMultiplier") ?? 4);
                settings.RingBufferSize = (int)(key.GetValue("RingBufferSize") ?? 8192);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Registry] Fehler beim Laden der WasapiBufferSettings: {ex.Message}");
            }

            return settings;
        }
    }
}
