using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

using Sound2Light.ViewModels.Units;

using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sound2Light.Views.Controls.Units
{
    public partial class UnitCapture : UserControl
    {
        private UnitCaptureViewModel? _viewModel;

        private const int LedCount = 48;
        private const float LedHeight = 6f;
        private const float LedSpacing = 2f;
        private const float LedWidth = 18f;
        private const float TotalHeight = LedCount * LedHeight + (LedCount - 1) * LedSpacing;

        public UnitCapture()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
            timer.Tick += (_, _) =>
            {
                LeftVuMeterCanvas.InvalidateVisual();
                RightVuMeterCanvas.InvalidateVisual();
                ScaleCanvas.InvalidateVisual();
            };
            timer.Start();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel = DataContext as UnitCaptureViewModel;
        }

        private void LeftVuMeterCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_viewModel == null) return;
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            DrawLeds(canvas, _viewModel.LeftActiveLed, _viewModel.LeftPeakLed, e.Info.Width, e.Info.Height);
        }

        private void RightVuMeterCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_viewModel == null) return;
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            DrawLeds(canvas, _viewModel.RightActiveLed, _viewModel.RightPeakLed, e.Info.Width, e.Info.Height);
        }

        private void ScaleCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();

            using var textPaint = new SKPaint
            {
                Color = SKColors.LightGray,
                IsAntialias = true,
                TextSize = 12,
                TextAlign = SKTextAlign.Center,
                Typeface = SKTypeface.FromFamilyName("Consolas")
            };

            string[] labels = new[] { "−∞", "−64", "−56", "−40", "−32", "−24", "−16", "−12", "−8", "−4", "0" };
            float[] dbValues = new[] { -72f, -64f, -56f, -40f, -32f, -24f, -16f, -12f, -8f, -4f, 0f };

            const float canvasWidth = 36f; // Breite der Spalte 1 manuell angeben
            float xPos = canvasWidth / 2f;

            for (int i = 0; i < labels.Length; i++)
            {
                int ledIndex = MapDbToLedIndex(dbValues[i]);
                float y = e.Info.Height - ledIndex * (LedHeight + LedSpacing) - LedHeight / 2f;
                if (labels[i] == "−∞") y += 3f; // kleine Korrektur für untere Position
                canvas.DrawText(labels[i], xPos, y, textPaint);
            }
        }

        // Diese Methode ist NUR für die Skala!
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

        /// <summary>
        /// Zeichnet die LEDs und legt Peak-LED als gelbe Linie drüber!
        /// </summary>
        private void DrawLeds(SKCanvas canvas, int activeIndex, int peakIndex, int width, int height)
        {
            using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

            for (int i = 0; i < LedCount; i++)
            {
                float y = height - i * (LedHeight + LedSpacing);
                paint.Color = i switch
                {
                    >= 46 => SKColors.Red,
                    >= 42 => new SKColor(255, 140, 0),   // Orange
                    _ => new SKColor(245, 245, 245)
                };
                if (i <= activeIndex)
                {
                    var rect = new SKRect(0, y - LedHeight, LedWidth, y);
                    canvas.DrawRect(rect, paint);
                }
            }

            // ---- PEAK-LED Overlay in der passenden Farbe ----
            if (peakIndex >= 0 && peakIndex < LedCount)
            {
                float yPeak = height - peakIndex * (LedHeight + LedSpacing) - LedHeight / 2f;
                SKColor peakColor = peakIndex switch
                {
                    >= 46 => SKColors.Red,
                    >= 42 => new SKColor(255, 140, 0),
                    _ => new SKColor(245, 245, 245)
                };
                using var peakPaint = new SKPaint
                {
                    Color = peakColor,
                    IsAntialias = true,
                    StrokeWidth = 4,
                    Style = SKPaintStyle.Stroke
                };
                canvas.DrawLine(0, yPeak, LedWidth, yPeak, peakPaint);
            }
        }
    }
}
