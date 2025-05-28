using System;
using System.Linq;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using Sound2Light.Models.Audio;

namespace Sound2Light.Services.Audio.Engines
{
    public class WasapiCaptureEngine : IAudioCaptureEngine
    {
        private WasapiCapture? _capture;
        private WaveFormat? _waveFormat;
        private BufferedWaveProvider? _bufferedProvider;
        private bool _isRunning;

        public event EventHandler<float[]>? AudioDataAvailable;
        public bool IsRunning => _isRunning;

        private readonly string _deviceName;

        public WasapiCaptureEngine(DeviceInfo device)
        {
            _deviceName = device.Name;
        }

        public void Start()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                var device = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                    .FirstOrDefault(d => d.FriendlyName == _deviceName);

                if (device == null)
                {
                    Console.WriteLine($"WASAPI-Gerät nicht gefunden: {_deviceName}");
                    return;
                }

                _capture = new WasapiCapture(device);
                _waveFormat = _capture.WaveFormat;
                _bufferedProvider = new BufferedWaveProvider(_waveFormat)
                {
                    DiscardOnBufferOverflow = true
                };

                _capture.DataAvailable += Capture_DataAvailable;
                _capture.StartRecording();
                _isRunning = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WASAPI Start fehlgeschlagen: {_deviceName} → {ex.Message}");
                _isRunning = false;
            }
        }

        public void Stop()
        {
            if (_capture != null)
            {
                _capture.DataAvailable -= Capture_DataAvailable;
                _capture.StopRecording();
                _capture.Dispose();
                _capture = null;
            }

            _isRunning = false;
        }

        private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_waveFormat == null || _waveFormat.BitsPerSample != 32 || _waveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                return;

            int sampleCount = e.BytesRecorded / 4 / _waveFormat.Channels;
            float[] monoBuffer = new float[sampleCount];

            // Konvertiere zu Mono (gemittelt)
            for (int i = 0; i < sampleCount; i++)
            {
                float sampleSum = 0;
                for (int ch = 0; ch < _waveFormat.Channels; ch++)
                {
                    int index = (i * _waveFormat.Channels + ch) * 4;
                    sampleSum += BitConverter.ToSingle(e.Buffer, index);
                }

                monoBuffer[i] = sampleSum / _waveFormat.Channels;
            }

            AudioDataAvailable?.Invoke(this, monoBuffer);
        }
    }
}
