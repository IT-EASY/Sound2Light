namespace Sound2Light.Models.Audio
{
    public enum AudioDeviceType
    {
        ASIO,
        WASAPI,
        DUMMY
    }

    public class DeviceInfo
    {
        /// <summary>
        /// ASIO oder WASAPI
        /// </summary>
        public AudioDeviceType Type { get; set; }

        /// <summary>
        /// Technischer Name: Treibername (ASIO) oder FriendlyName (WASAPI)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Anzeigename für UI, z. B. "[ASIO] UR22C"
        /// </summary>
        public string DisplayName => $"[{Type}] {Name}";

        /// <summary>
        /// Gibt an, ob das Gerät aktuell verfügbar/nutzbar ist
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Bevorzugte Abtastrate laut Treiber
        /// </summary>
        public int? PreferredSampleRate { get; set; }

        /// <summary>
        /// Bevorzugte Bittiefe laut Treiber oder System
        /// </summary>
        public int? PreferredBitDepth { get; set; }

        /// <summary>
        /// Anzahl Eingänge laut Treiber oder Format
        /// </summary>
        public int? PreferredInputChannels { get; set; }

        /// <summary>
        /// Bevorzugte Buffergröße (nur bei ASIO bekannt)
        /// </summary>
        public int? PreferredBufferSize { get; set; }
    }
}
