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
            @"Software\Sound2Light\RingBuffers\MonoAnalysis",
            @"Software\Sound2Light\Net\sACN",
            @"Software\Sound2Light\Net\sACN\DMX-Mapping",
            @"Software\Sound2Light\Net\artNET",
            @"Software\Sound2Light\Net\artNET\DMX-Mapping"
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
            
            // Netzwerk-Protokoll-Defaults
            if (keyPath.EndsWith(@"Net\sACN"))
            {
                using var sacnKey = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
                if (sacnKey != null)
                {
                    EnsureValue(sacnKey, "Enabled", 0); // aus = 0
                    EnsureValue(sacnKey, "Universe", 10);
                }
            }
            if (keyPath.EndsWith(@"Net\artNET"))
            {
                using var artnetKey = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
                if (artnetKey != null)
                {
                    EnsureValue(artnetKey, "Enabled", 0);
                    EnsureValue(artnetKey, "Universe", 11);
                }
            }
            if (keyPath.EndsWith(@"DMX-Mapping"))
            {
                using var mappingKey = Registry.CurrentUser.OpenSubKey(keyPath, writable: true);
                if (mappingKey != null)
                {
                    string[] defaultOutputs = new[]
                    {
                        "BPM", "S2L-Low", "S2L-Low-Middle", "S2L-Middle", "S2L-High", "B2L-Main", "B2L-STEM"
                    };
                    foreach (var output in defaultOutputs)
                    {
                        EnsureValue(mappingKey, output, "n/a");
                    }
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
