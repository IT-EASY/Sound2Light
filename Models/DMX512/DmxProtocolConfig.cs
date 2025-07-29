using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sound2Light.Models.DMX512
{
    /// <summary>
    /// Konfiguration eines DMX-Protokolls (ArtNet, sACN).
    /// </summary>
    public class DmxProtocolConfig : INotifyPropertyChanged
    {
        private bool _enabled;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _universe;
        public int Universe
        {
            get => _universe;
            set
            {
                if (_universe != value)
                {
                    _universe = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
