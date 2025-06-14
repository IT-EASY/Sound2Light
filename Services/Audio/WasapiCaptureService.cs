using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;

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
        private bool _isRunning;

        // Callback/Event für Buffer-Daten (hier: float[] interleaved, length = frameCount*2)
        public event Action<float[]>? SamplesAvailable;

        public WasapiCaptureService(AudioDevice device, int bufferSize)
        {
            _device = device;
            _bufferSize = bufferSize;
        }

        public void Start()
        {
            if (_isRunning)
                return;

            // Suche das Device anhand des Namens (der vom System gelieferten FriendlyName)
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

            // CSCore-WASAPI-Capture initialisieren
            _capture = new WasapiCapture
            {
                Device = mmDevice,
                // Optional: BufferSize anpassen (in Bytes)
                // BufferSize = _bufferSize, // meist nicht kritisch, CSCore macht das automatisch
            };

            _capture.Initialize();

            // Format prüfen und ggf. konvertieren auf 32-bit-float stereo
            var source = new SoundInSource(_capture)
                .ToSampleSource()
                .ToStereo(); // falls Mono → Stereo

            // .ToSampleSource() gibt 32bit-float interleaved aus

            // WaveSource für die Eventverarbeitung zwischenspeichern
            _waveSource = source.ToWaveSource();

            // Event-Handler
            _capture.DataAvailable += (s, e) =>
            {
                // Buffer für ein Frame anlegen (frame = ein Sample pro Kanal)
                var buffer = new float[_bufferSize * 2]; // Stereo = 2 Samples pro Frame

                int read = source.Read(buffer, 0, buffer.Length);

                if (read > 0)
                {
                    // Optional: Debug-Ausgabe
                    // Debug.WriteLine($"[WASAPI] {read} Samples gelesen.");

                    // Event feuern oder an Buffer-Service weitergeben
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
            catch { /* Ignorieren, falls schon gestoppt */ }
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
