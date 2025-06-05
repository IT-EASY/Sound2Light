using Microsoft.Win32;

using Sound2Light.Models.Audio;
using Sound2Light.Services.System;

using System;
using System.Collections.Generic;

namespace Sound2Light.Services.Devices
{
    public class AsioDetectionService : IAsioDetectionService
    {
        private const string AsioRegistryPath = @"SOFTWARE\ASIO";

        public bool IsAsioAvailable()
        {
            using var key = Registry.LocalMachine.OpenSubKey(AsioRegistryPath);
            return key?.SubKeyCount > 0;
        }

        public List<AsioDriverReference> GetAvailableDrivers()
        {
            var result = new List<AsioDriverReference>();
            using var root = Registry.LocalMachine.OpenSubKey(AsioRegistryPath);
            if (root == null) return result;

            foreach (var subKeyName in root.GetSubKeyNames())
            {
                using var subKey = root.OpenSubKey(subKeyName);
                if (subKey == null) continue;

                var clsidStr = subKey.GetValue("CLSID") as string;
                if (string.IsNullOrWhiteSpace(clsidStr)) continue;

                if (!Guid.TryParse(clsidStr, out var clsid)) continue;

                result.Add(new AsioDriverReference
                {
                    Name = subKeyName, // oder (string)subKey.GetValue("Description") ?? subKeyName
                    Clsid = clsid
                });
            }

            return result;
        }
    }
}
