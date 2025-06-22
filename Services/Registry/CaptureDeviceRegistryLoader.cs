using Microsoft.Win32;

using Sound2Light.Models.Audio;

using System;
using System.Diagnostics;

namespace Sound2Light.Services.RegistryAccess
{
    public static class CaptureDeviceRegistryLoader
    {
        private const string BasePath = @"Software\Sound2Light\CaptureDevices";

        public static AudioDevice? LoadPreferredDevice()
        {
            return LoadDeviceFromRegistry("PreferredCaptureDevice");
        }

        public static AudioDevice? LoadCurrentDevice()
        {
            return LoadDeviceFromRegistry("CurrentCaptureDevice");
        }

        private static AudioDevice? LoadDeviceFromRegistry(string subKeyName)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey($@"{BasePath}\{subKeyName}");
                if (key == null)
                {
                    Debug.WriteLine($"[Registry] {subKeyName} nicht gefunden.");
                    return null;
                }

                var device = new AudioDevice
                {
                    Name = key.GetValue("Name")?.ToString() ?? string.Empty,
                    Type = (AudioDeviceType)(int)(key.GetValue("CaptureDeviceType") ?? 0),
                    SampleRate = (int)(key.GetValue("SampleRate") ?? 0),
                    BitDepth = (int)(key.GetValue("BitDepth") ?? 0),
                    ChannelCount = (int)(key.GetValue("ChannelCount") ?? 0),
                    BufferSize = (int)(key.GetValue("BufferSize") ?? 0),
                    SampleType = (int)(key.GetValue("SampleType") ?? 0),
                    //IsAvailable = true // wird später ggf. via DeviceCheck angepasst
                };

                return device;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Registry] Fehler beim Laden von {subKeyName}: {ex.Message}");
                return null;
            }
        }
    }
}
