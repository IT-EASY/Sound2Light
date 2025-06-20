using AsioBridgeCLI;

using CommunityToolkit.Mvvm.ComponentModel;

using Sound2Light.Services.Audio;

using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace Sound2Light.ViewModels.Units
{
    public class UnitCaptureViewModel : ObservableObject
    {
        private readonly AsioCaptureService _asioService;
        private readonly DispatcherTimer _timer;

        // Konfiguration
        private const int LedCount = 48;
        private const int FramesPerUpdate = 512;
        private const int PeakHoldMilliseconds = 200;
        private const int PeakDecayMilliseconds = 80;
        private const int SampleRate = 48000;

        // Anzeige-Properties
        public int LeftActiveLed { get; private set; }
        public int RightActiveLed { get; private set; }
        public int LeftPeakLed { get; private set; }
        public int RightPeakLed { get; private set; }

        // Peak-Hold/Decay-Status
        private DateTime _leftPeakSetTime = DateTime.MinValue;
        private DateTime _rightPeakSetTime = DateTime.MinValue;
        private bool _leftDecayActive = false;
        private bool _rightDecayActive = false;

        public UnitCaptureViewModel(AsioCaptureService asioService)
        {
            _asioService = asioService;
            // 20 Hz entspricht 50ms, optimal für VU-Meter
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(FramesPerUpdate * 1000.0 / SampleRate) };
            _timer.Tick += (s, e) => UpdateLevels();
            _timer.Start();
        }

        private void UpdateLevels()
        {
            float[] left = new float[FramesPerUpdate];
            float[] right = new float[FramesPerUpdate];

            bool gotSamples = false;

            if (_asioService.HasNewSamplesFor("VuMeter", FramesPerUpdate))
            {
                var handleLeft = System.Runtime.InteropServices.GCHandle.Alloc(left, System.Runtime.InteropServices.GCHandleType.Pinned);
                var handleRight = System.Runtime.InteropServices.GCHandle.Alloc(right, System.Runtime.InteropServices.GCHandleType.Pinned);

                _asioService.CopySamplesTo("VuMeter", handleLeft.AddrOfPinnedObject(), handleRight.AddrOfPinnedObject(), FramesPerUpdate);
                gotSamples = true;

                handleLeft.Free();
                handleRight.Free();
            }

            if (gotSamples)
            {
                float maxL = 0, maxR = 0;
                for (int i = 0; i < FramesPerUpdate; i++)
                {
                    maxL = Math.Max(maxL, Math.Abs(left[i]));
                    maxR = Math.Max(maxR, Math.Abs(right[i]));
                }
                float dbL = maxL > 0 ? 20f * (float)Math.Log10(maxL) : -72f;
                float dbR = maxR > 0 ? 20f * (float)Math.Log10(maxR) : -72f;

                int ledL = MapDbToLedIndex(dbL);
                int ledR = MapDbToLedIndex(dbR);

                LeftActiveLed = ledL;
                RightActiveLed = ledR;

                // --------- Peak Hold / Decay: Links ---------
                if (ledL > LeftPeakLed)
                {
                    LeftPeakLed = ledL;
                    _leftPeakSetTime = DateTime.UtcNow;
                    _leftDecayActive = false;
                }
                else if (ledL == LeftPeakLed)
                {
                    _leftPeakSetTime = DateTime.UtcNow;
                    _leftDecayActive = false;
                }
                else
                {
                    var now = DateTime.UtcNow;
                    if (!_leftDecayActive && (now - _leftPeakSetTime).TotalMilliseconds > PeakHoldMilliseconds)
                    {
                        _leftDecayActive = true;
                        _leftPeakSetTime = now;
                    }
                    if (_leftDecayActive && (now - _leftPeakSetTime).TotalMilliseconds > PeakDecayMilliseconds)
                    {
                        LeftPeakLed = Math.Max(LeftPeakLed - 1, ledL);
                        _leftPeakSetTime = now;
                        if (LeftPeakLed == ledL)
                            _leftDecayActive = false;
                    }
                }

                // --------- Peak Hold / Decay: Rechts ---------
                if (ledR > RightPeakLed)
                {
                    RightPeakLed = ledR;
                    _rightPeakSetTime = DateTime.UtcNow;
                    _rightDecayActive = false;
                }
                else if (ledR == RightPeakLed)
                {
                    _rightPeakSetTime = DateTime.UtcNow;
                    _rightDecayActive = false;
                }
                else
                {
                    var now = DateTime.UtcNow;
                    if (!_rightDecayActive && (now - _rightPeakSetTime).TotalMilliseconds > PeakHoldMilliseconds)
                    {
                        _rightDecayActive = true;
                        _rightPeakSetTime = now;
                    }
                    if (_rightDecayActive && (now - _rightPeakSetTime).TotalMilliseconds > PeakDecayMilliseconds)
                    {
                        RightPeakLed = Math.Max(RightPeakLed - 1, ledR);
                        _rightPeakSetTime = now;
                        if (RightPeakLed == ledR)
                            _rightDecayActive = false;
                    }
                }
            }
            // Bei fehlenden Daten (gotSamples == false) keine Änderung der Anzeige!

            OnPropertyChanged(nameof(LeftActiveLed));
            OnPropertyChanged(nameof(RightActiveLed));
            OnPropertyChanged(nameof(LeftPeakLed));
            OnPropertyChanged(nameof(RightPeakLed));
        }

        // Skala/Mapping exakt wie im Control!
        private int MapDbToLedIndex(float db)
        {
            if (db <= -72f) return 0;
            if (db >= 4f) return 47;

            if (db < -40f)
            {
                float fraction = (db + 72f) / 32f;
                return (int)(fraction * 6);
            }
            else if (db < -4f)
            {
                float fraction = (db + 40f) / 36f;
                return 6 + (int)(fraction * 37);
            }
            else
            {
                // High: −4 dB bis +4 dB auf LEDs 43–47 (5 LEDs, 1.6 dB/LED)
                float fraction = (db + 4f) / 8f;
                return 43 + (int)(fraction * 5);
            }
        }
    }
}
