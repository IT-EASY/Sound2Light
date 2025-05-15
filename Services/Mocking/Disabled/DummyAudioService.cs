// Sound2Light/Services/Mocking/DummyAudioService.cs
using Sound2Light.Services.Audio;
using Sound2Light.Settings;

using System;
using System.Threading;

namespace Sound2Light.Services.Mocking.Disabled
{
    public class DummyAudioService : IAudioService
    {
        private readonly AppSettings _settings;
        private Timer? _timer;
        private readonly Random _rand = new();

        public AudioRingBuffer Buffer { get; }
        public event Action<float[]>? DataAvailable;

        public DummyAudioService(AppSettings settings)
        {
            _settings = settings;
            Buffer = new AudioRingBuffer(settings);
        }

        public void Start()
        {
            _timer = new Timer(_ => GenerateData(), null, 0, 100); // 10 Hz
        }

        private void GenerateData()
        {
            var samples = new float[_settings.Audio.SampleRate / 10];
            // Testdaten generieren (z.B. Sinuswelle)
            Buffer.Append(samples);
            DataAvailable?.Invoke(samples);
        }

        public void Stop() => _timer?.Dispose();
    }
}