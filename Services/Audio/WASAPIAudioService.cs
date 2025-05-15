// Sound2Light/Services/Audio/WASAPIAudioService.cs
using NAudio.CoreAudioApi;
using NAudio.Wave;

using Sound2Light.Common;
using Sound2Light.Models;

using System;

using static Sound2Light.Common.ErrorEventArgs;

namespace Sound2Light.Services.Audio
{
    public class WASAPIAudioService : IAudioService, IDisposable
    {
        private WasapiCapture? _capture;
        private AudioDeviceInfo? _device;
        private bool _disposed;
        private bool _isInitialized;

        public event EventHandler<AudioDataEventArgs>? DataAvailable;
        public event EventHandler<ErrorEventArgs>? ErrorOccurred;

        public void Initialize(AudioDeviceInfo device, AudioSettings settings)
        {
            if (_isInitialized) return;

            try
            {
                _device = device;
                var enumerator = new MMDeviceEnumerator();
                var mmDevice = enumerator.GetDevice(device.TechnicalName);

                _capture = new WasapiCapture(mmDevice)
                {
                    WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(
                        settings.SampleRate,
                        settings.Channels)
                };

                _capture.DataAvailable += OnDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs
                {
                    ErrorCode = ErrorCodes.WASAPI_INIT_FAILED,
                    Message = $"WASAPI-Init: {ex.Message}"
                });
                Dispose();
            }
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (e.BytesRecorded % 4 != 0)
                {
                    ErrorOccurred?.Invoke(this, new ErrorEventArgs
                    {
                        ErrorCode = ErrorCodes.AUDIO_PROCESSING_ERROR,
                        Message = "Ungültige Byte-Länge für Float32"
                    });
                    return;
                }

                var buffer = new float[e.BytesRecorded / 4];
                Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

                DataAvailable?.Invoke(this, new AudioDataEventArgs
                {
                    Buffer = buffer,
                    DeviceInfo = _device!,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs
                {
                    ErrorCode = ErrorCodes.AUDIO_PROCESSING_ERROR,
                    Message = $"Datenkonvertierung: {ex.Message}"
                });
            }
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs
                {
                    ErrorCode = ErrorCodes.WASAPI_RECORDING_FAILED,
                    Message = $"Aufnahmeabbruch: {e.Exception.Message}"
                });
            }
        }

        public void Start() => _capture?.StartRecording();
        public void Stop() => _capture?.StopRecording();

        public void Dispose()
        {
            if (_disposed) return;
            _capture?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this); // Wichtig für korrektes Dispose-Muster
        }
    }
}