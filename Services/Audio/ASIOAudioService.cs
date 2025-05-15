// Sound2Light/Services/Audio/ASIOAudioService.cs  
using NAudio.Wave;
using NAudio.Wave.Asio;
using Sound2Light.Common;
using Sound2Light.Models;
using System;
using static Sound2Light.Common.ErrorEventArgs;

namespace Sound2Light.Services.Audio
{
    public class ASIOAudioService : IAudioService, IDisposable
    {
        private AsioOut? _asioOut;
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
                _asioOut = new AsioOut(device.TechnicalName)
                {
                    InputChannelOffset = device.ChannelOffset
                };

                if (!_asioOut.IsSampleRateSupported(settings.SampleRate))
                    throw new ArgumentException($"SampleRate {settings.SampleRate}Hz nicht unterstützt");

                _asioOut.InitRecordAndPlayback(null, settings.Channels, settings.SampleRate);
                _asioOut.AudioAvailable += OnAudioAvailable;
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new ErrorEventArgs
                {
                    ErrorCode = ErrorCodes.ASIO_DRIVER_INIT_FAILED,
                    Message = $"ASIO-Init: {ex.Message}"
                });
                Dispose();
            }
        }

        private void OnAudioAvailable(object? sender, AsioAudioAvailableEventArgs e)
        {
            try
            {
                var buffer = new float[e.SamplesPerBuffer * e.InputBuffers.Length];
                e.GetAsInterleavedSamples(buffer);

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
                    Message = $"Audio-Datenfehler: {ex.Message}"
                });
            }
        }

        public void Start() => _asioOut?.Play();
        public void Stop() => _asioOut?.Stop();

        public void Dispose()
        {
            if (_disposed) return;
            _asioOut?.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this); // Wichtig für korrektes Dispose-Muster
        }
    }
}