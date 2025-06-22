using System.Collections.Generic;

namespace Sound2Light.Models.DMX512
{
    /// <summary>
    /// Komplette DMX-Mapping-Konfiguration (für appsettings.json).
    /// </summary>
    public class DmxMappingConfig
    {
        /// <summary>
        /// sACN-Protokoll-Config.
        /// </summary>
        public DmxProtocolConfig Sacn { get; set; } = new();

        /// <summary>
        /// ArtNet-Protokoll-Config.
        /// </summary>
        public DmxProtocolConfig ArtNet { get; set; } = new();

        /// <summary>
        /// Die eigentlichen Zuordnungen von Outputs zu Kanälen.
        /// </summary>
        public List<DmxOutputMapping> Outputs { get; set; } = new();
    }
}
