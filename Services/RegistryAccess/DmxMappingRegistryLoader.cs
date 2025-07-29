using Microsoft.Win32;

using Sound2Light.Models.DMX512;

using System;
using System.Collections.Generic;

namespace Sound2Light.Services.RegistryAccess
{
    public static class DmxMappingRegistryLoader
    {
        private const string SacnBaseKey = @"Software\Sound2Light\Net\sACN";
        private const string ArtNetBaseKey = @"Software\Sound2Light\Net\artNET";

        public static DmxMappingConfig LoadDmxMapping()
        {
            var config = new DmxMappingConfig
            {
                Sacn = LoadProtocolConfig(SacnBaseKey),
                ArtNet = LoadProtocolConfig(ArtNetBaseKey),
                Outputs = LoadOutputMappings(SacnBaseKey, ArtNetBaseKey)
            };

            return config;
        }

        private static DmxProtocolConfig LoadProtocolConfig(string baseKeyPath)
        {
            using var key = Registry.CurrentUser.OpenSubKey(baseKeyPath);
            if (key == null)
                return new DmxProtocolConfig();

            var enabled = Convert.ToInt32(key.GetValue("Enabled", 0)) != 0;
            var universe = Convert.ToInt32(key.GetValue("Universe", 0));

            return new DmxProtocolConfig
            {
                Enabled = enabled,
                Universe = universe
            };
        }

        private static List<DmxOutputMapping> LoadOutputMappings(string sacnBaseKey, string artnetBaseKey)
        {
            var mappings = new List<DmxOutputMapping>();
            using var sacnMapKey = Registry.CurrentUser.OpenSubKey($@"{sacnBaseKey}\DMX-Mapping");
            using var artMapKey = Registry.CurrentUser.OpenSubKey($@"{artnetBaseKey}\DMX-Mapping");

            var outputNames = new HashSet<string>();
            if (sacnMapKey != null)
                foreach (var name in sacnMapKey.GetValueNames())
                    outputNames.Add(name);
            if (artMapKey != null)
                foreach (var name in artMapKey.GetValueNames())
                    outputNames.Add(name);

            foreach (var name in outputNames)
            {
                int? sacn = ParseChannel(sacnMapKey?.GetValue(name));
                int? art = ParseChannel(artMapKey?.GetValue(name));

                mappings.Add(new DmxOutputMapping
                {
                    OutputName = name,
                    SacnChannel = sacn,
                    ArtNetChannel = art
                });
            }

            return mappings;
        }

        private static int? ParseChannel(object? value)
        {
            if (value == null) return null;
            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str) || str.Equals("n/a", StringComparison.OrdinalIgnoreCase))
                return null;

            return int.TryParse(str, out int result) ? result : null;
        }
    }
}
