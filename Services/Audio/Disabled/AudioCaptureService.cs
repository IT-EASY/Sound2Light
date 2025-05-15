using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// AudioCaptureService.cs (abgespeckte, aber funktionsfähige Version)
using Sound2Light.Settings;

namespace Sound2Light.Services.Audio.Disabled
{
    public class AudioCaptureService : IAudioService
    {
        private readonly AppSettings _settings;
        private Timer? _timer;
        private readonly Random _rand = new();

        public AudioRingBuffer Buffer { get; }
        public event Action<float[]>? DataAvailable;

        public AudioCaptureService(AppSettings settings)
        {
            _settings = settings;
            Buffer = new AudioRingBuffer(settings);
        }

        public void Start()
        {
            // Grundlegende Implementierung ohne Hardware-Bindung
            _timer = new Timer(_ => GenerateDummyData(), null, 0, 100); // 10 Hz
        }

        private void GenerateDummyData()
        {
            // Einfache Testdaten generieren (ähnlich DummyService)
            var samples = new float[_settings.Audio.SampleRate / 10]; // 100ms
            Array.Fill(samples, 0.5f); // Konstanter Pegel
            Buffer.Append(samples);
            DataAvailable?.Invoke(samples);
        }

        public void Stop() => _timer?.Dispose();
    }
}