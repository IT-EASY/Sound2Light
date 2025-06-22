using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Sound2Light.Config;
using Sound2Light.Helpers.UI;
using Sound2Light.Models.DMX512;

namespace Sound2Light.ViewModels.Windows
{
    public class SetupDMXViewModel : INotifyPropertyChanged
    {
        private readonly IAppConfigurationService _config;
        private readonly Action? _closeCallback;

        public DmxProtocolConfig SacnConfig { get; }
        public DmxProtocolConfig ArtNetConfig { get; }

        public ObservableCollection<DmxOutputMappingViewModel> OutputMappings { get; }

        public ObservableCollection<int?> AvailableSacnChannels { get; }
        public ObservableCollection<int?> AvailableArtNetChannels { get; }

        // Commands for Buttons
        public ICommand SetCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CloseCommand { get; }

        public SetupDMXViewModel(
            IAppConfigurationService config,
            Action? closeCallback = null)
        {
            _config = config;
            _closeCallback = closeCallback;

            // Get from config or default
            var mappingConfig = _config.Settings.DmxMapping ?? CreateDefaultConfig();

            SacnConfig = mappingConfig.Sacn;
            ArtNetConfig = mappingConfig.ArtNet;

            // Create shared channel lists (0/n/a + 1-512)
            AvailableSacnChannels = new ObservableCollection<int?>(GetChannelList());
            AvailableArtNetChannels = new ObservableCollection<int?>(GetChannelList());

            // Wrap OutputMappings as ViewModels
            OutputMappings = new ObservableCollection<DmxOutputMappingViewModel>(
                mappingConfig.Outputs.Select(o => new DmxOutputMappingViewModel(o, AvailableSacnChannels, AvailableArtNetChannels, OnChannelChanged))
            );

            SetCommand = new RelayCommand(_ => SaveAndClose());
            ClearCommand = new RelayCommand(_ => ClearAllMappings());
            CloseCommand = new RelayCommand(_ => _closeCallback?.Invoke());
        }

        // Channel-Liste: null für n/a, dann 1-512
        private static ObservableCollection<int?> GetChannelList()
        {
            var list = new ObservableCollection<int?> { null }; // "n/a"
            for (int i = 1; i <= 512; i++) list.Add(i);
            return list;
        }

        // Wird aufgerufen, wenn ein Channel geändert wurde – entfernt/fügt Channels in den Auswahllisten hinzu/weg
        private void OnChannelChanged()
        {
            // Pro Protokoll die belegten Channels erfassen
            var usedSacn = OutputMappings.Select(vm => vm.SacnChannel).Where(c => c.HasValue && c > 0).ToHashSet();
            var usedArtNet = OutputMappings.Select(vm => vm.ArtNetChannel).Where(c => c.HasValue && c > 0).ToHashSet();

            // Für jede Zeile: Die verfügbaren Channels neu setzen
            foreach (var vm in OutputMappings)
            {
                vm.RefreshAvailableChannels(usedSacn, usedArtNet);
            }
        }

        // Save Mapping to Config and close window
        private void SaveAndClose()
        {
            // Schreibe OutputMappings in Settings
            var mappingConfig = new DmxMappingConfig
            {
                Sacn = this.SacnConfig,
                ArtNet = this.ArtNetConfig,
                Outputs = OutputMappings.Select(vm => vm.ToModel()).ToList()
            };
            _config.Settings.DmxMapping = mappingConfig;
            _config.SaveConfiguration();
            _closeCallback?.Invoke();
        }

        // Alle Zuweisungen löschen ("n/a")
        private void ClearAllMappings()
        {
            foreach (var vm in OutputMappings)
            {
                vm.SacnChannel = null;
                vm.ArtNetChannel = null;
            }
            OnChannelChanged();
        }

        // Falls kein Mapping geladen, mit Default-Outputs erzeugen:
        private static DmxMappingConfig CreateDefaultConfig()
        {
            return new DmxMappingConfig
            {
                Sacn = new DmxProtocolConfig { Enabled = false, Universe = 0 },
                ArtNet = new DmxProtocolConfig { Enabled = false, Universe = 0 },
                Outputs = new System.Collections.Generic.List<DmxOutputMapping>
                {
                    new DmxOutputMapping { OutputName = "Indikator" },
                    new DmxOutputMapping { OutputName = "FFT-Low" },
                    new DmxOutputMapping { OutputName = "FFT-Low-Middle" },
                    new DmxOutputMapping { OutputName = "FFT-Middle" },
                    new DmxOutputMapping { OutputName = "FFT-High" },
                    new DmxOutputMapping { OutputName = "Beat2Light_OnSet" },
                    new DmxOutputMapping { OutputName = "Beat2Light_Cluster" },
                    new DmxOutputMapping { OutputName = "Beat2Light_STEM" },
                    new DmxOutputMapping { OutputName = "BPM" }
                }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Einzelne Zeile, inkl. Logik für dynamische Channel-Auswahlliste
    public class DmxOutputMappingViewModel : INotifyPropertyChanged
    {
        private readonly DmxOutputMapping _model;
        private readonly ObservableCollection<int?> _sacnChannels;
        private readonly ObservableCollection<int?> _artNetChannels;
        private readonly Action _onChannelChanged;

        private ObservableCollection<int?> _myAvailableSacnChannels = new();
        private ObservableCollection<int?> _myAvailableArtNetChannels = new();

        public DmxOutputMappingViewModel(
            DmxOutputMapping model,
            ObservableCollection<int?> sacnChannels,
            ObservableCollection<int?> artNetChannels,
            Action onChannelChanged)
        {
            _model = model;
            _sacnChannels = sacnChannels;
            _artNetChannels = artNetChannels;
            _onChannelChanged = onChannelChanged;
            OutputName = model.OutputName;
            SacnChannel = model.SacnChannel;
            ArtNetChannel = model.ArtNetChannel;
            // Init eigene Channel-Listen
            _myAvailableSacnChannels = new ObservableCollection<int?>(_sacnChannels);
            _myAvailableArtNetChannels = new ObservableCollection<int?>(_artNetChannels);
        }

        public string OutputName { get; }

        private int? _sacnChannel;
        public int? SacnChannel
        {
            get => _sacnChannel;
            set
            {
                if (_sacnChannel != value)
                {
                    _sacnChannel = value;
                    _model.SacnChannel = value;
                    OnPropertyChanged();
                    _onChannelChanged?.Invoke();
                }
            }
        }

        private int? _artNetChannel;
        public int? ArtNetChannel
        {
            get => _artNetChannel;
            set
            {
                if (_artNetChannel != value)
                {
                    _artNetChannel = value;
                    _model.ArtNetChannel = value;
                    OnPropertyChanged();
                    _onChannelChanged?.Invoke();
                }
            }
        }

        // Für dynamische Sperrung (aktuell: Alle Channels immer verfügbar, kann erweitert werden)
        public ObservableCollection<int?> AvailableSacnChannels => _myAvailableSacnChannels;
        public ObservableCollection<int?> AvailableArtNetChannels => _myAvailableArtNetChannels;

        // Aufruf nach jeder Änderung, um die verfügbaren Channels neu zu setzen
        public void RefreshAvailableChannels(System.Collections.Generic.ISet<int?> usedSacn, System.Collections.Generic.ISet<int?> usedArtNet)
        {
            UpdateChannelList(_sacnChannel, _sacnChannels, _myAvailableSacnChannels, usedSacn);
            UpdateChannelList(_artNetChannel, _artNetChannels, _myAvailableArtNetChannels, usedArtNet);
        }

        private static void UpdateChannelList(int? ownValue, ObservableCollection<int?> allChannels, ObservableCollection<int?> targetList, System.Collections.Generic.ISet<int?> used)
        {
            targetList.Clear();
            foreach (var ch in allChannels)
            {
                if (ch == null || ch == ownValue || !used.Contains(ch))
                    targetList.Add(ch);
            }
            // Immer sicherstellen, dass der aktuelle Wert auswählbar bleibt
            if (ownValue.HasValue && !targetList.Contains(ownValue))
                targetList.Add(ownValue);
        }

        public DmxOutputMapping ToModel()
        {
            _model.SacnChannel = SacnChannel;
            _model.ArtNetChannel = ArtNetChannel;
            return _model;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
