using Microsoft.Win32;

using Sound2Light.Models.Audio;
using Sound2Light.Settings;

using System.Diagnostics;

namespace Sound2Light.Services.RegistryAccess
{
    public static class RingBufferRegistryWriter
    {
        private const string WasapiPath = @"Software\Sound2Light\RingBuffers\WasapiDevice";

        public static void SaveWasapiSettings(RingBufferSettings settings)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(WasapiPath);
                if (key == null)
                {
                    Debug.WriteLine("[Registry] Fehler: RingBuffer-Schlüssel (Wasapi) konnte nicht erstellt werden.");
                    return;
                }

                key.SetValue("LatencyMultiplier", settings.LatencyMultiplier);
                key.SetValue("RingBufferSize", settings.RingBufferSize);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Registry] Fehler beim Schreiben der WasapiBufferSettings: {ex.Message}");
            }
        }
    }
}
