using NAudio.Wave;
using NAudio.Wave.Asio;

using Sound2Light.Models.Audio;

using System;
using System.Runtime.InteropServices;

namespace Sound2Light.Services.Audio.Engines
{
    public class AsioCaptureEngine : IAudioCaptureEngine
    {
        private readonly string _driverName;
        private readonly int _sampleRate;
        private readonly int _channelCount;
        private AsioOut? _asioOut;
        private bool _isRunning;

        public event EventHandler<float[]>? AudioDataAvailable;
        public bool IsRunning => _isRunning;

        public AsioCaptureEngine(DeviceInfo device)
        {
            _driverName = device.Name;
            _sampleRate = device.PreferredSampleRate ?? 44100;
            _channelCount = device.PreferredInputChannels ?? 2;
        }

        public void Start()
        {
            try
            {
                _asioOut = new AsioOut(_driverName);
                _asioOut.InputChannelOffset = 0;

                _asioOut.InitRecordAndPlayback(
                    new SilenceWaveProvider(_sampleRate, _channelCount),
                    _channelCount,
                    _sampleRate);

                _asioOut.AudioAvailable += AsioOut_AudioAvailable;
                _asioOut.Play();
                _isRunning = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ASIO Capture Start failed: {_driverName} → {ex.Message}");
                _isRunning = false;
            }
        }

        public void Stop()
        {
            if (_asioOut != null)
            {
                _asioOut.AudioAvailable -= AsioOut_AudioAvailable;
                _asioOut.Stop();
                _asioOut.Dispose();
                _asioOut = null;
            }

            _isRunning = false;
        }

        private void AsioOut_AudioAvailable(object? sender, AsioAudioAvailableEventArgs e)
        {
            var buffer = new float[e.SamplesPerBuffer];
            float invChCount = 1f / _channelCount;

            for (int ch = 0; ch < _channelCount; ch++)
            {
                IntPtr ptr = e.InputBuffers[ch];
                float[] channelData = new float[e.SamplesPerBuffer];
                Marshal.Copy(ptr, channelData, 0, e.SamplesPerBuffer);

                for (int i = 0; i < e.SamplesPerBuffer; i++)
                {
                    buffer[i] += channelData[i] * invChCount;
                }
            }

            AudioDataAvailable?.Invoke(this, buffer);
        }

        private class SilenceWaveProvider : IWaveProvider
        {
            public WaveFormat WaveFormat { get; }

            public SilenceWaveProvider(int sampleRate, int channels)
            {
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                Array.Clear(buffer, offset, count);
                return count;
            }
        }
    }
}
