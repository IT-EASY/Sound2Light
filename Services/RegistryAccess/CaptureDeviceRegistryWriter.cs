using Microsoft.Win32;

using Sound2Light.Models.Audio;

using System.Diagnostics;

namespace Sound2Light.Services.RegistryAccess
{
    public static class CaptureDeviceRegistryWriter
    {
        private const string BasePath = @"Software\Sound2Light\CaptureDevices";

        public static void SavePreferredDevice(AudioDevice device)
        {
            SaveDeviceToRegistry("PreferredCaptureDevice", device);
        }

        public static void SaveCurrentDevice(AudioDevice device)
        {
            SaveDeviceToRegistry("CurrentCaptureDevice", device);
        }

        private static void SaveDeviceToRegistry(string subKeyName, AudioDevice device)
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey($@"{BasePath}\{subKeyName}");
                if (key == null)
                {
                    Debug.WriteLine($"[Registry] Fehler: Konnte '{subKeyName}' nicht öffnen.");
                    return;
                }

                key.SetValue("Name", device.Name);
                key.SetValue("CaptureDeviceType", (int)device.Type);
                key.SetValue("SampleRate", device.SampleRate);
                key.SetValue("BitDepth", device.BitDepth);
                key.SetValue("ChannelCount", device.ChannelCount);
                key.SetValue("BufferSize", device.BufferSize ?? 0);
                key.SetValue("SampleType", device.SampleType ?? 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Registry] Fehler beim Schreiben von {subKeyName}: {ex.Message}");
            }
        }
    }
}
