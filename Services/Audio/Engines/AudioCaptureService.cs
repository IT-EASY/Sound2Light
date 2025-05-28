using Sound2Light.Models.Audio;
using Sound2Light.Settings;
using Sound2Light.Services.Audio.Engines;
using System;

namespace Sound2Light.Services.Audio
{
    public class AudioCaptureService
    {
        private readonly AppSettings _settings;
        private IAudioCaptureEngine? _engine;

        public event EventHandler<float[]>? AudioDataAvailable;

        public bool IsRunning => _engine?.IsRunning ?? false;

        public AudioCaptureService(AppSettings settings)
        {
            _settings = settings;
        }

        public void StartCapture(DeviceInfo device)
        {
            StopCapture();

            _engine = CreateEngine(device);

            if (_engine != null)
            {
                _engine.AudioDataAvailable += (s, data) => AudioDataAvailable?.Invoke(this, data);
                _engine.Start();
            }
        }

        public void StopCapture()
        {
            _engine?.Stop();
            _engine = null;
        }

        private IAudioCaptureEngine? CreateEngine(DeviceInfo device)
        {
            return device.Type switch
            {
                AudioDeviceType.ASIO => new AsioCaptureEngine(device),
                AudioDeviceType.WASAPI => new WasapiCaptureEngine(device),
                AudioDeviceType.DUMMY => new DummyCaptureEngine(device),
                _ => null
            };
        }
    }
}
