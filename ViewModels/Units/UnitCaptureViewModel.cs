using AsioBridgeCLI;

using CommunityToolkit.Mvvm.ComponentModel;

using Sound2Light.Contracts.Audio;
using Sound2Light.Models.Audio;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Sound2Light.ViewModels.Units
{
    public class UnitCaptureViewModel : ObservableObject
    {
        private readonly DispatcherTimer _timer;

        private AudioDeviceType _deviceType;
        private AsioCaptureService? _asioService;
        private IWasapiRingBuffer? _wasapiBuffer;

        private const int FramesPerUpdate = 512;
        private const int SampleRate = 48000;
        private const int PeakHoldMilliseconds = 200;
        private const int PeakDecayMilliseconds = 80;
        private const int LedCount = 48;

        private int _leftActiveLed;
        public int LeftActiveLed
        {
            get => _leftActiveLed;
            private set => SetProperty(ref _leftActiveLed, value);
        }

        private int _rightActiveLed;
        public int RightActiveLed
        {
            get => _rightActiveLed;
            private set => SetProperty(ref _rightActiveLed, value);
        }

        private int _leftPeakLed;
        public int LeftPeakLed
        {
            get => _leftPeakLed;
            private set => SetProperty(ref _leftPeakLed, value);
        }

        private int _rightPeakLed;
        public int RightPeakLed
        {
            get => _rightPeakLed;
            private set => SetProperty(ref _rightPeakLed, value);
        }

        private DateTime _leftPeakSetTime = DateTime.MinValue;
        private DateTime _rightPeakSetTime = DateTime.MinValue;
        private bool _leftDecayActive = false;
        private bool _rightDecayActive = false;

        public UnitCaptureViewModel()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(FramesPerUpdate * 1000.0 / SampleRate) };
            _timer.Tick += (s, e) => UpdateLevels();
            _timer.Start();
        }

        public void SetCaptureSource(AudioDeviceType deviceType)
        {
            _deviceType = deviceType;

            if (deviceType == AudioDeviceType.Asio)
            {
                _asioService = ((App)App.Current).Services.GetService(typeof(AsioCaptureService)) as AsioCaptureService;
                Debug.WriteLine("[UnitCaptureViewModel] ASIO Service zugewiesen.");
            }
            else if (deviceType == AudioDeviceType.Wasapi)
            {
                _wasapiBuffer = ((App)App.Current).Services.GetService(typeof(IWasapiRingBuffer)) as IWasapiRingBuffer;
                Debug.WriteLine("[UnitCaptureViewModel] WASAPI Buffer zugewiesen.");
            }
            Debug.WriteLine($"[UnitCaptureViewModel] Capture Source gesetzt: {_deviceType}");

        }
        private void UpdateLevels()
        {
            float[] left = new float[FramesPerUpdate];
            float[] right = new float[FramesPerUpdate];
            bool gotSamples = false;

            if (_deviceType == AudioDeviceType.Asio && _asioService != null && _asioService.HasNewSamplesFor("AsioVuMeter", FramesPerUpdate))
            {
                var hL = GCHandle.Alloc(left, GCHandleType.Pinned);
                var hR = GCHandle.Alloc(right, GCHandleType.Pinned);
                _asioService.CopySamplesTo("AsioVuMeter", hL.AddrOfPinnedObject(), hR.AddrOfPinnedObject(), FramesPerUpdate);
                hL.Free();
                hR.Free();
                gotSamples = true;
            }
            else if (_deviceType == AudioDeviceType.Wasapi && _wasapiBuffer != null)
            {
                bool hasNew = _wasapiBuffer.HasNewSamplesFor("WasapiVuMeter", FramesPerUpdate);
                //Debug.WriteLine($"[VU] HAS new samples: {hasNew}");

                if (hasNew)
                {
                    gotSamples = _wasapiBuffer.CopySamplesFor("WasapiVuMeter", left, right, FramesPerUpdate);
                    // Debug.WriteLine($"[VU] CopySamplesFor returned: {gotSamples}");
                }
            }

            if (!gotSamples)
            {
                //Debug.WriteLine($"[VU] CopySamplesFor returned: {gotSamples}");
                return;
            }
            //return;

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

            UpdatePeak(ref _leftPeakLed, ledL, ref _leftPeakSetTime, ref _leftDecayActive);
            UpdatePeak(ref _rightPeakLed, ledR, ref _rightPeakSetTime, ref _rightDecayActive);
        }

        private void UpdatePeak(ref int peakLed, int currentLed, ref DateTime peakSetTime, ref bool decayActive)
        {
            var now = DateTime.UtcNow;

            if (currentLed > peakLed)
            {
                peakLed = currentLed;
                peakSetTime = now;
                decayActive = false;
            }
            else if (currentLed == peakLed)
            {
                peakSetTime = now;
                decayActive = false;
            }
            else
            {
                if (!decayActive && (now - peakSetTime).TotalMilliseconds > PeakHoldMilliseconds)
                {
                    decayActive = true;
                    peakSetTime = now;
                }
                if (decayActive && (now - peakSetTime).TotalMilliseconds > PeakDecayMilliseconds)
                {
                    peakLed = Math.Max(peakLed - 1, currentLed);
                    peakSetTime = now;
                    if (peakLed == currentLed)
                        decayActive = false;
                }
            }
        }

        private int MapDbToLedIndex(float db)
        {
            if (db <= -72f) return 0;
            if (db >= 4f) return LedCount - 1;

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
                float fraction = (db + 4f) / 8f;
                return 43 + (int)(fraction * 5);
            }
        }
    }
}
