namespace Sound2Light.Contracts.Services.Audio
{
    /// <summary>
    /// Interface für alle Analyse-Buffer (egal ob WASAPI, ASIO oder andere).
    /// </summary>
    public interface IAudioAnalysisBuffer
    {
        /// <summary>
        /// Liefert die letzten count Frames als Left/Right, false falls nicht genug Samples.
        /// </summary>
        bool TryGetLatestFrames(float[] left, float[] right, int frameCount);
    }
}
