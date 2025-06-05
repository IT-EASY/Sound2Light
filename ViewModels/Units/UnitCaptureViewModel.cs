using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

public partial class UnitCaptureViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private int leftActiveLed;

    [ObservableProperty]
    private int rightActiveLed;

    private const int LedCount = 48;

    public UnitCaptureViewModel()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(33)
        };
        _timer.Tick += OnTimerTick;
       // _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        // 👇 Dummy-Samples: Sinuswelle mit leicht variierter Phase
        var time = DateTime.Now.TimeOfDay.TotalSeconds;
        float left = (float)Math.Sin(time * 2 * Math.PI * 1.2);
        float right = (float)Math.Sin(time * 2 * Math.PI * 1.3);

        float peakLeft = MathF.Abs(left);
        float peakRight = MathF.Abs(right);

        LeftActiveLed = MapAmplitudeToLedIndex(peakLeft);
        RightActiveLed = MapAmplitudeToLedIndex(peakRight);

        Debug.WriteLine($"Left Amplitude: {peakLeft:F3}, LED: {LeftActiveLed}");
    }

    private int MapAmplitudeToLedIndex(float amplitude)
    {
        if (amplitude <= 0f)
            return 0;

        double db = 20.0 * Math.Log10(amplitude);

        if (db <= -72.0)
            return 0;
        if (db >= +6.0)
            return LedCount - 1;

        if (db <= -40.0)
        {
            double ratio = (db + 72.0) / 32.0;
            return (int)(ratio * 12);
        }
        else
        {
            double ratio = (db + 40.0) / 46.0;
            return 12 + (int)(ratio * 36);
        }
    }
}
