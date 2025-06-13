using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sound2Light.ViewModels.Windows
{
    public class EngineSetupViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // FFT-Size Optionen
        public int[] FFTSizes { get; } = new[] { 256, 512, 1024, 2048, 4096 };
        private int _selectedFFTSize = 1024;
        public int SelectedFFTSize
        {
            get => _selectedFFTSize;
            set
            {
                if (_selectedFFTSize != value)
                {
                    _selectedFFTSize = value;
                    OnPropertyChanged();
                }
            }
        }

        // Buffer-Multiplier Optionen
        public int[] BufferMultipliers { get; } = new[] { 1, 2, 4, 8 };
        private int _selectedBufferMultiplier = 2;
        public int SelectedBufferMultiplier
        {
            get => _selectedBufferMultiplier;
            set
            {
                if (_selectedBufferMultiplier != value)
                {
                    _selectedBufferMultiplier = value;
                    OnPropertyChanged();
                }
            }
        }

        // Beispiel für weitere Analyse-Parameter:
        // private double _sensitivity = 1.0;
        // public double Sensitivity
        // {
        //     get => _sensitivity;
        //     set
        //     {
        //         if (_sensitivity != value)
        //         {
        //             _sensitivity = value;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // Weitere Properties und Logik können hier ergänzt werden...
    }
}
