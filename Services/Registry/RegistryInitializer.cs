using System.Diagnostics;

using Microsoft.Win32;

namespace Sound2Light.Services.RegistryAccess
{
    public static class RegistryInitializer
    {
        private static readonly string[] RequiredKeys =
        {
            @"Software\Sound2Light",
            @"Software\Sound2Light\CaptureDevices",
            @"Software\Sound2Light\CaptureDevices\CurrentCaptureDevice",
            @"Software\Sound2Light\CaptureDevices\PreferredCaptureDevice",
            @"Software\Sound2Light\RingBuffers",
            @"Software\Sound2Light\RingBuffers\WasapiDevice",
            @"Software\Sound2Light\RingBuffers\MonoAnalysis"
        };

        public static void EnsureRegistryStructure()
        {
            foreach (var keyPath in RequiredKeys)
            {
                CreateKeyAndEnsureValues(keyPath);
            }
        }

        private static void CreateKeyAndEnsureValues(string keyPath)
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);

            if (key == null)
            {
                using var created = Registry.CurrentUser.CreateSubKey(keyPath);
                Debug.WriteLine($"[Registry] Erstellt: {keyPath}");
            }
            else
            {
                Debug.WriteLine($"[Registry] OK: {keyPath}");
            }

            // CaptureDevice-Werte
            if (keyPath.EndsWith("CurrentCaptureDevice") || keyPath.EndsWith("PreferredCaptureDevice"))
            {
                using var deviceKey = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
                if (deviceKey != null)
                {
                    EnsureValue(deviceKey, "Name", "");
                    EnsureValue(deviceKey, "CaptureDeviceType", 0); // 0 = ASIO
                    EnsureValue(deviceKey, "SampleRate", 44100);
                    EnsureValue(deviceKey, "BitDepth", 24);
                    EnsureValue(deviceKey, "ChannelCount", 2);
                    EnsureValue(deviceKey, "BufferSize", 512);
                    EnsureValue(deviceKey, "SampleType", 19);
                }
            }

            // RingBuffer-Werte (nur für WasapiDevice & MonoAnalysis)
            if (keyPath.EndsWith("WasapiDevice") || keyPath.EndsWith("MonoAnalysis"))
            {
                using var bufferKey = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
                if (bufferKey != null)
                {
                    EnsureValue(bufferKey, "LatencyMultiplier", 8);
                    EnsureValue(bufferKey, "RingBufferSize", 4096);
                }
            }
        }

        private static void EnsureValue(RegistryKey key, string name, object defaultValue)
        {
            if (key.GetValue(name) == null)
            {
                key.SetValue(name, defaultValue);
                Debug.WriteLine($"[Registry] → Property '{name}' ergänzt mit Default = {defaultValue}");
            }
        }
    }
}
