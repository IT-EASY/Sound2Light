using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Sound2Light.Config;
using Sound2Light.Services.Audio;
using Sound2Light.Settings;
using Sound2Light.Models.Audio;

namespace Sound2Light.Services.Audio
{
    public class MonoFeederService : IDisposable
    {
        private readonly IAppConfigurationService _config;
        private readonly MonoAnalysisBuffer _monoBuffer;
        private readonly WasapiRingBuffer? _wasapiBuffer;
        private readonly AsioBridgeCLI.AsioCaptureService? _asioService;
        private readonly Timer _timer;

        private const string ConsumerName = "MonoBuffer";
        private const int FillSize = 512;
        private readonly float[] _left = new float[FillSize];
        private readonly float[] _right = new float[FillSize];

        private AudioDeviceType? _lastDeviceType = null;
        private string? _lastDeviceName = null;

        public MonoFeederService(
            IAppConfigurationService config,
            MonoAnalysisBuffer monoBuffer,
            WasapiRingBuffer? wasapiBuffer,
            AsioBridgeCLI.AsioCaptureService? asioService)
        {
            _config = config;
            _monoBuffer = monoBuffer;
            _wasapiBuffer = wasapiBuffer;
            _asioService = asioService;

            _monoBuffer.RegisterConsumer(ConsumerName);

            var device = _config.Settings.CurrentDevice;
            if (device != null && device.Type == AudioDeviceType.Wasapi && _wasapiBuffer != null)
            {
                TryRegisterWasapiConsumer(_wasapiBuffer, ConsumerName);
            }
            else if (device != null && device.Type == AudioDeviceType.Asio && _asioService != null)
            {
                _asioService.RegisterConsumer(ConsumerName);
            }

            _timer = new Timer(OnTick, null, 0, 33);
        }

        private void OnTick(object? state)
        {
            var device = _config.Settings.CurrentDevice;
            if (device == null) return;

            bool ok = false;
            if (device.Type == AudioDeviceType.Wasapi && _wasapiBuffer != null)
            {
                bool hasNew = _wasapiBuffer.HasNewSamplesFor(ConsumerName, FillSize);
                if (hasNew)
                {
                    ok = _wasapiBuffer.CopySamplesFor(ConsumerName, _left, _right, FillSize);
                }
            }
            else if (device.Type == AudioDeviceType.Asio && _asioService != null)
            {
                if (_asioService.HasNewSamplesFor(ConsumerName, FillSize))
                {
                    var hLeft = GCHandle.Alloc(_left, GCHandleType.Pinned);
                    var hRight = GCHandle.Alloc(_right, GCHandleType.Pinned);
                    try
                    {
                        _asioService.CopySamplesTo(ConsumerName, hLeft.AddrOfPinnedObject(), hRight.AddrOfPinnedObject(), FillSize);
                        ok = true;
                    }
                    finally
                    {
                        hLeft.Free();
                        hRight.Free();
                    }
                }
            }

            if (ok)
            {
                _monoBuffer.FillFromStereoBuffer_EnergyPreserving(_left, _right, FillSize);

                float peak = 0f;
                for (int i = 0; i < FillSize; i++)
                {
                    // Energieerhaltender Mono-Mix für die Peak-Berechnung (rms + Vorzeichen)
                    float mono = MathF.Sqrt((_left[i] * _left[i] + _right[i] * _right[i]) / 2f) * MathF.Sign(_left[i] + _right[i]);
                    if (Math.Abs(mono) > peak) peak = Math.Abs(mono);
                }

                if (_lastDeviceType != device.Type || _lastDeviceName != device.Name)
                {
                    Debug.WriteLine($"[MonoFeeder] Aktiv: {device.Type} ({device.Name})");
                    _lastDeviceType = device.Type;
                    _lastDeviceName = device.Name;
                }

                // Debug.WriteLine($"[MonoFeeder] Device: {device.Type}, Peak (MonoBlock): {peak:0.000}");
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _monoBuffer.UnregisterConsumer(ConsumerName);

            var device = _config.Settings.CurrentDevice;
            if (device != null && device.Type == AudioDeviceType.Wasapi && _wasapiBuffer != null)
            {
                TryUnregisterWasapiConsumer(_wasapiBuffer, ConsumerName);
            }
            else if (device != null && device.Type == AudioDeviceType.Asio && _asioService != null)
            {
                _asioService.UnregisterConsumer(ConsumerName);
            }
        }

        private static void TryRegisterWasapiConsumer(WasapiRingBuffer buffer, string name)
        {
            var mi = buffer.GetType().GetMethod("RegisterConsumer");
            mi?.Invoke(buffer, new object[] { name });
        }
        private static void TryUnregisterWasapiConsumer(WasapiRingBuffer buffer, string name)
        {
            var mi = buffer.GetType().GetMethod("UnregisterConsumer");
            mi?.Invoke(buffer, new object[] { name });
        }
    }
}
