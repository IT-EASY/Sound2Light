namespace Sound2Light.Models.Audio;

public class RingBufferSettings
{
    /// <summary>
    /// Größe des Ringpuffers in Anzahl Samples (nicht Bytes).
    /// </summary>
    public int RingBufferSize { get; set; } = 4096;
    /// <summary>
    /// Multiplikator auf Gerätebuffergröße (z. B. 1.5 → 150 %).
    /// </summary>
    public double LatencyMultiplier { get; set; } = 2.0;

}
