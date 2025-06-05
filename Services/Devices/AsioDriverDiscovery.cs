using Sound2Light.Contracts.Services.Devices;
using Sound2Light.Models.Audio;
using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Sound2Light.Services.Devices
{
    public class AsioDriverDiscovery : IAsioDriverDiscovery
    {
        public List<AsioDriverReference> GetAsioDriverReferences()
        {
            var drivers = new List<AsioDriverReference>();

            using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var asioKey = localMachine.OpenSubKey(@"SOFTWARE\ASIO");

            if (asioKey == null)
                return drivers;

            foreach (var subKeyName in asioKey.GetSubKeyNames())
            {
                using var driverKey = asioKey.OpenSubKey(subKeyName);
                if (driverKey == null) continue;

                string? clsidStr = driverKey.GetValue("CLSID") as string;
                string? description = driverKey.GetValue("Description") as string;

                if (string.IsNullOrWhiteSpace(clsidStr)) continue;

                if (!Guid.TryParse(clsidStr, out Guid clsid)) continue;

                var reference = new AsioDriverReference
                {
                    Name = description ?? subKeyName,
                    Clsid = clsid,
                    DllPath = GetDllPathFromClsid(clsidStr)
                };

                drivers.Add(reference);
            }

            return drivers;
        }

        private string? GetDllPathFromClsid(string clsid)
        {
            try
            {
                var path = $@"CLSID\{clsid}\InprocServer32";
                using var clsidKey = Registry.ClassesRoot.OpenSubKey(path);
                return clsidKey?.GetValue(null) as string;
            }
            catch
            {
                return null;
            }
        }
    }
}
