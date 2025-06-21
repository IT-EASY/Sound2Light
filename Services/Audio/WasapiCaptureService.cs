using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;

using Sound2Light.Contracts.Audio;
using Sound2Light.Models.Audio;

using System;
using System.Diagnostics;

namespace Sound2Light.Services.Audio
{
    public class WasapiCaptureService : IDisposable
    {
        private WasapiCapture? _capture;
        private IWaveSource? _waveSource;
        private readonly AudioDevice _device;
        private readonly int _bufferSize;
        private readonly IWasapiRingBuffer _ringBuffer;
        private bool _isRunning;

        public event Action<float[]>? SamplesAvailable;

        public WasapiCaptureService(AudioDevice device, int bufferSize, IWasapiRingBuffer ringBuffer)
        {
            _device = device;
            _bufferSize = bufferSize;
            _ringBuffer = ringBuffer;
        }

        public void Start()
        {
            if (_isRunning)
                return;

            MMDevice? mmDevice = null;
            using (var enumerator = new MMDeviceEnumerator())
            {
                foreach (var dev in enumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active))
                {
                    if (dev.FriendlyName == _device.Name)
                    {
                        mmDevice = dev;
                        break;
                    }
                    dev.Dispose();
                }
            }

            if (mmDevice == null)
                throw new InvalidOperationException("Device nicht gefunden: " + _device.Name);

            _capture = new WasapiCapture
            {
                Device = mmDevice,
            };

            _capture.Initialize();

            var source = new SoundInSource(_capture)
                .ToSampleSource()
                .ToStereo();

            _waveSource = source.ToWaveSource();

            _capture.DataAvailable += (s, e) =>
            {
                var buffer = new float[_bufferSize * 2];
                int read = source.Read(buffer, 0, buffer.Length);

                if (read > 0)
                {
                    _ringBuffer.Write(buffer, 0, read / 2); // Stereo → Frames = Samples / 2
                    SamplesAvailable?.Invoke(buffer);
                }
                else
                {
                    Debug.WriteLine("[WasapiCaptureService] Kein Sample gelesen!");
                }
            };

            _capture.Start();
            _isRunning = true;

            Debug.WriteLine("[WASAPI] CaptureService gestartet.");
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            try
            {
                _capture?.Stop();
            }
            catch { }
            _isRunning = false;
        }

        public void Dispose()
        {
            Stop();
            _waveSource?.Dispose();
            _capture?.Dispose();
        }
    }
}
