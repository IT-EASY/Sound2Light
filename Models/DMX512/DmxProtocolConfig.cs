namespace Sound2Light.Models.DMX512
{
    /// <summary>
    /// Konfiguration eines DMX-Protokolls (ArtNet, sACN).
    /// </summary>
    public class DmxProtocolConfig
    {
        /// <summary>
        /// Gibt an, ob das Protokoll aktiv genutzt wird.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Verwendetes Universe (0-65535).
        /// </summary>
        public int Universe { get; set; } = 0;
    }
}
