using System;
using System.Threading;

using Sound2Light.Models.Audio;

namespace Sound2Light.Services.Audio.Engines
{
    public class DummyCaptureEngine : IAudioCaptureEngine
    {
        private readonly int _sampleRate;
        private readonly int _channelCount;
        private Timer? _timer;
        private bool _isRunning;

        public event EventHandler<float[]>? AudioDataAvailable;

        public bool IsRunning => _isRunning;

        public DummyCaptureEngine(DeviceInfo device)
        {
            _sampleRate = device.PreferredSampleRate ?? 44100;
            _channelCount = device.PreferredInputChannels ?? 2;
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _timer = new Timer(GenerateDummyData, null, 0, 100); // 10 Hz
        }

        private void GenerateDummyData(object? state)
        {
            int samplesPerBuffer = _sampleRate / 10; // 100ms Buffer
            var buffer = new float[samplesPerBuffer];

            // Fülle mit konstantem Pegel (kann später geändert werden)
            for (int i = 0; i < samplesPerBuffer; i++)
                buffer[i] = 0.5f;

            AudioDataAvailable?.Invoke(this, buffer);
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
            _isRunning = false;
        }
    }
}
