using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Sound2Light.Services.UI;
using Sound2Light.Helpers.UI;

namespace Sound2Light.ViewModels.Main
{
    public class PowerButtonViewModel : INotifyPropertyChanged
    {
        private readonly IPowerButtonStateService _stateService;

        public PowerButtonViewModel(IPowerButtonStateService stateService)
        {
            _stateService = stateService;

            _stateService.UnlockStateChanged += (_, unlocked) =>
            {
                IsUnlocked = unlocked;
            };

            IsUnlocked = _stateService.IsUnlocked;

            StartUnlockCommand = new RelayCommand(_ => _stateService.StartUnlockDirect());
            AttemptShutdownCommand = new RelayCommand(_ => _stateService.AttemptShutdown());
        }

        private bool _isUnlocked;
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                if (_isUnlocked != value)
                {
                    _isUnlocked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ButtonText));
                    OnPropertyChanged(nameof(LockIcon));
                    OnPropertyChanged(nameof(GlowColor));
                    OnPropertyChanged(nameof(ButtonCenterColor));
                    OnPropertyChanged(nameof(ButtonEdgeColor));
                    OnPropertyChanged(nameof(BorderBrushDynamic));
                    OnPropertyChanged(nameof(GlowRadius));
                }
            }
        }

        public string ButtonText => "ON"; // bleibt konstant
        public string LockIcon => IsUnlocked ? "🔓" : "🔒";

        public SolidColorBrush GlowColor =>
            IsUnlocked
                ? new SolidColorBrush(Color.FromRgb(0xBB, 0x33, 0x33)) // rot
                : new SolidColorBrush(Color.FromRgb(0x66, 0xCC, 0x66)); // grün

        public Color ButtonCenterColor =>
            IsUnlocked
                ? Color.FromRgb(0x99, 0x00, 0x00)
                : Color.FromRgb(0x1A, 0xFF, 0x1A);

        public Color ButtonEdgeColor =>
            IsUnlocked
                ? Color.FromRgb(0x22, 0x00, 0x00)
                : Color.FromRgb(0x00, 0x33, 0x00);

        public Brush BorderBrushDynamic =>
            IsUnlocked
                ? new LinearGradientBrush(Color.FromRgb(0xAA, 0x33, 0x33), Color.FromRgb(0x44, 0x00, 0x00), new Point(0, 0), new Point(0, 1))
                : new LinearGradientBrush(Color.FromRgb(0x55, 0xAA, 0x55), Color.FromRgb(0x00, 0x44, 0x00), new Point(0, 0), new Point(0, 1));

        public double GlowRadius => IsUnlocked ? 12.0 : 6.0;

        public ICommand StartUnlockCommand { get; }
        public ICommand AttemptShutdownCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
