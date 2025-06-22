namespace Sound2Light.Models.DMX512
{
    /// <summary>
    /// Ein Mapping eines Sound2Light-Outputs auf je einen sACN- und ArtNet-Channel.
    /// Channel = null oder -1 bedeutet "n/a" (nicht senden).
    /// </summary>
    public class DmxOutputMapping
    {
        /// <summary>
        /// Logischer Name des Outputs, z. B. "FFT-Low", "Beat2Light_OnSet".
        /// </summary>
        public string OutputName { get; set; } = string.Empty;

        /// <summary>
        /// DMX-Kanal (1–512) für sACN, null oder -1 = nicht verwendet.
        /// </summary>
        public int? SacnChannel { get; set; } = null;

        /// <summary>
        /// DMX-Kanal (1–512) für ArtNet, null oder -1 = nicht verwendet.
        /// </summary>
        public int? ArtNetChannel { get; set; } = null;
    }
}
