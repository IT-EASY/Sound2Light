namespace Sound2Light.Services.Audio.Engines
{
    public interface IAudioCaptureEngine
    {
        void Start();
        void Stop();
        bool IsRunning { get; }
        event EventHandler<float[]>? AudioDataAvailable;
    }
}
