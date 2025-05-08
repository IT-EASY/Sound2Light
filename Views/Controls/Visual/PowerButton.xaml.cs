using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sound2Light.Views.Controls.Visual
{
    public partial class PowerButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly DispatcherTimer _unlockTimer;
        private readonly DispatcherTimer _relockTimer;
        private bool _isLocked = true;

        private SolidColorBrush _glowColor;
        public SolidColorBrush GlowColor
        {
            get => _glowColor;
            set { _glowColor = value; OnPropertyChanged(nameof(GlowColor)); }
        }

        private Color _buttonCenterColor;
        public Color ButtonCenterColor
        {
            get => _buttonCenterColor;
            set { _buttonCenterColor = value; OnPropertyChanged(nameof(ButtonCenterColor)); }
        }

        private Color _buttonEdgeColor;
        public Color ButtonEdgeColor
        {
            get => _buttonEdgeColor;
            set { _buttonEdgeColor = value; OnPropertyChanged(nameof(ButtonEdgeColor)); }
        }

        private Brush _borderBrushDynamic;
        public Brush BorderBrushDynamic
        {
            get => _borderBrushDynamic;
            set { _borderBrushDynamic = value; OnPropertyChanged(nameof(BorderBrushDynamic)); }
        }

        public PowerButton()
        {
            InitializeComponent();

            GlowColor = new SolidColorBrush(Color.FromRgb(0x66, 0xCC, 0x66));
            ButtonCenterColor = Color.FromRgb(0x1A, 0xFF, 0x1A);
            ButtonEdgeColor = Color.FromRgb(0x00, 0x33, 0x00);
            BorderBrushDynamic = new LinearGradientBrush(
                Color.FromRgb(0x55, 0xAA, 0x55),
                Color.FromRgb(0x00, 0x44, 0x00),
                new Point(0, 0), new Point(0, 1));

            DataContext = this;

            _unlockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _unlockTimer.Tick += UnlockTimer_Tick;

            _relockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _relockTimer.Tick += RelockTimer_Tick;

            MouseRightButtonDown += OnRightMouseDown;
            MouseRightButtonUp += OnRightMouseUp;
            MouseLeftButtonDown += OnLeftMouseDown;
        }

        private void OnRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isLocked) _unlockTimer.Start();
        }

        private void OnRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            _unlockTimer.Stop();
        }

        private void OnLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isLocked)
            {
                Application.Current.Shutdown();
            }
        }

        private void UnlockTimer_Tick(object? sender, EventArgs e)
        {
            _unlockTimer.Stop();
            SetUnlockedState(true);
            _relockTimer.Start();
        }

        private void RelockTimer_Tick(object? sender, EventArgs e)
        {
            _relockTimer.Stop();
            SetUnlockedState(false);
        }

        private void SetUnlockedState(bool unlocked)
        {
            _isLocked = !unlocked;

            if (unlocked)
            {
                GlowColor = new SolidColorBrush(Color.FromRgb(0xBB, 0x33, 0x33));
                ButtonCenterColor = Color.FromRgb(0x99, 0x00, 0x00);
                ButtonEdgeColor = Color.FromRgb(0x22, 0x00, 0x00);
                BorderBrushDynamic = new LinearGradientBrush(
                    Color.FromRgb(0xAA, 0x33, 0x33),
                    Color.FromRgb(0x44, 0x00, 0x00),
                    new Point(0, 0), new Point(0, 1));
                LockIcon.Text = "🔓";
            }
            else
            {
                GlowColor = new SolidColorBrush(Color.FromRgb(0x66, 0xCC, 0x66));
                ButtonCenterColor = Color.FromRgb(0x1A, 0xFF, 0x1A);
                ButtonEdgeColor = Color.FromRgb(0x00, 0x33, 0x00);
                BorderBrushDynamic = new LinearGradientBrush(
                    Color.FromRgb(0x55, 0xAA, 0x55),
                    Color.FromRgb(0x00, 0x44, 0x00),
                    new Point(0, 0), new Point(0, 1));
                LockIcon.Text = "🔒";
            }

            OnPropertyChanged(nameof(GlowColor));
            OnPropertyChanged(nameof(ButtonCenterColor));
            OnPropertyChanged(nameof(ButtonEdgeColor));
            OnPropertyChanged(nameof(BorderBrushDynamic));
        }
    }
}
