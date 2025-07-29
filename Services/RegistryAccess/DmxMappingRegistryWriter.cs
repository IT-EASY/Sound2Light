using Microsoft.Win32;

using Sound2Light.Models.DMX512;

using System.Collections.Generic;

namespace Sound2Light.Services.RegistryAccess
{
    public static class DmxMappingRegistryWriter
    {
        private const string SacnBaseKey = @"Software\Sound2Light\Net\sACN";
        private const string ArtNetBaseKey = @"Software\Sound2Light\Net\artNET";

        public static void SaveDmxMapping(DmxMappingConfig config)
        {
            SaveProtocolConfig(SacnBaseKey, config.Sacn);
            SaveProtocolConfig(ArtNetBaseKey, config.ArtNet);

            SaveOutputMappings($@"{SacnBaseKey}\DMX-Mapping", config.Outputs, useSacn: true);
            SaveOutputMappings($@"{ArtNetBaseKey}\DMX-Mapping", config.Outputs, useSacn: false);
        }

        private static void SaveProtocolConfig(string baseKeyPath, DmxProtocolConfig protocol)
        {
            using var key = Registry.CurrentUser.CreateSubKey(baseKeyPath);
            if (key == null) return;

            key.SetValue("Enabled", protocol.Enabled ? 1 : 0);
            key.SetValue("Universe", protocol.Universe);
        }

        private static void SaveOutputMappings(string mappingKeyPath, List<DmxOutputMapping> outputs, bool useSacn)
        {
            using var key = Registry.CurrentUser.CreateSubKey(mappingKeyPath, writable: true);
            if (key == null) return;

            foreach (var output in outputs)
            {
                var channel = useSacn ? output.SacnChannel : output.ArtNetChannel;
                key.SetValue(output.OutputName, channel.HasValue && channel.Value > 0
                    ? channel.Value.ToString()
                    : "n/a");
            }
        }
    }
}
