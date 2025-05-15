// NAudioWrapper.cs
using NAudio.CoreAudioApi;
using NAudio.Wave;

using Sound2Light.Settings;

namespace Sound2Light.Services.Audio.Disabled
{
    internal class NAudioWrapper : IDisposable
    {
        private WasapiCapture? _capture;
        private readonly AppSettings _settings;

        public NAudioWrapper(AppSettings settings)
        {
            _settings = settings;
        }

        public void Start()
        {
            _capture = new WasapiCapture();
            _capture.DataAvailable += OnDataAvailable;
            _capture.StartRecording();
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            // Rohdatenverarbeitung hier
        }

        public void Dispose() => _capture?.Dispose();
    }
}