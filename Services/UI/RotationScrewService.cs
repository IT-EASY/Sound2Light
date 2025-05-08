// Services/UI/RotationScrewService.cs
using System;
using System.Windows;
using System.Windows.Media;

namespace Sound2Light.Services.UI
{
    public static class RotationScrewService
    {
        public static readonly DependencyProperty InitializeRandomRotationProperty =
            DependencyProperty.RegisterAttached(
                "InitializeRandomRotation",
                typeof(bool),
                typeof(RotationScrewService),
                new PropertyMetadata(false, new PropertyChangedCallback(OnInitializeRotationChanged)));

        public static bool GetInitializeRandomRotation(DependencyObject obj)
            => (bool)obj.GetValue(InitializeRandomRotationProperty);

        public static void SetInitializeRandomRotation(DependencyObject obj, bool value)
            => obj.SetValue(InitializeRandomRotationProperty, value);

        private static void OnInitializeRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && (bool)e.NewValue)
            {
                element.Loaded += (s, _) =>
                {
                    var rnd = new Random();
                    element.RenderTransform = new RotateTransform(rnd.Next(0, 360));
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
                };
            }
        }
    }
}