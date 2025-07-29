using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Sound2Light.Config;
using Sound2Light.Helpers.UI;
using Sound2Light.Models.DMX512;

namespace Sound2Light.ViewModels.Addon
{
    public enum NumpadTarget
    {
        None,
        SacnChannel,
        ArtNetChannel,
        SacnUniverse,
        ArtNetUniverse
    }

    public class SetupDMXViewModel : INotifyPropertyChanged
    {
        private readonly IAppConfigurationService _config;
        private readonly Action? _closeCallback;

        public DmxProtocolConfig SacnConfig { get; }
        public DmxProtocolConfig ArtNetConfig { get; }

        public ObservableCollection<DmxOutputMappingViewModel> OutputMappings { get; }

        private string _numpadInput = "";
        public string NumpadInput
        {
            get => _numpadInput;
            set { _numpadInput = value; OnPropertyChanged(); }
        }

        private NumpadTarget _activeNumpadTarget = NumpadTarget.None;
        public NumpadTarget ActiveNumpadTarget
        {
            get => _activeNumpadTarget;
            set { _activeNumpadTarget = value; OnPropertyChanged(); }
        }

        private DmxOutputMappingViewModel? _selectedSacnOutput;
        public DmxOutputMappingViewModel? SelectedSacnOutput
        {
            get => _selectedSacnOutput;
            set
            {
                if (_selectedSacnOutput != value)
                {
                    _selectedSacnOutput = value;
                    if (value != null)
                    {
                        ActiveNumpadTarget = NumpadTarget.SacnChannel;
                        NumpadInput = value.SacnChannel?.ToString() ?? "";
                    }
                    OnPropertyChanged();
                }
            }
        }

        private DmxOutputMappingViewModel? _selectedArtNetOutput;
        public DmxOutputMappingViewModel? SelectedArtNetOutput
        {
            get => _selectedArtNetOutput;
            set
            {
                if (_selectedArtNetOutput != value)
                {
                    _selectedArtNetOutput = value;
                    if (value != null)
                    {
                        ActiveNumpadTarget = NumpadTarget.ArtNetChannel;
                        NumpadInput = value.ArtNetChannel?.ToString() ?? "";
                    }
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ChannelDigitButton> NumpadButtons { get; }

        public ICommand NumpadDigitCommand { get; }
        public ICommand NumpadClearCommand { get; }
        public ICommand NumpadSetCommand { get; }
        public ICommand SetCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ClearSacnChannelCommand { get; }
        public ICommand ClearArtNetChannelCommand { get; }

        // Neue Commands für das Universe-Numpad-Target
        public ICommand SelectSacnUniverseTargetCommand { get; }
        public ICommand SelectArtNetUniverseTargetCommand { get; }

        public SetupDMXViewModel(
            IAppConfigurationService config,
            Action? closeCallback = null)
        {
            _config = config;
            _closeCallback = closeCallback;

            var mappingConfig = _config.Settings.DmxMapping;
            if (mappingConfig == null || mappingConfig.Outputs == null || mappingConfig.Outputs.Count == 0)
            {
                mappingConfig = CreateDefaultConfig();
                _config.Settings.DmxMapping = mappingConfig;
            }

            SacnConfig = mappingConfig.Sacn;
            ArtNetConfig = mappingConfig.ArtNet;

            SacnConfig.PropertyChanged += OnProtocolConfigChanged;
            ArtNetConfig.PropertyChanged += OnProtocolConfigChanged;

            OutputMappings = new ObservableCollection<DmxOutputMappingViewModel>();
            foreach (var model in mappingConfig.Outputs)
            {
                OutputMappings.Add(new DmxOutputMappingViewModel(model, OutputMappings, OnChannelChanged));
            }

            SetCommand = new RelayCommand(_ => SaveAndClose());
            ClearCommand = new RelayCommand(_ => ClearAllMappings());
            CloseCommand = new RelayCommand(_ => _closeCallback?.Invoke());

            NumpadDigitCommand = new RelayCommand(param => AddDigit(param as string));
            NumpadClearCommand = new RelayCommand(_ => NumpadInput = "");
            NumpadSetCommand = new RelayCommand(_ => AssignNumpadInput());

            ClearSacnChannelCommand = new RelayCommand(param => ClearSacnChannel(param));
            ClearArtNetChannelCommand = new RelayCommand(param => ClearArtNetChannel(param));

            SelectSacnUniverseTargetCommand = new RelayCommand(_ => { ActiveNumpadTarget = NumpadTarget.SacnUniverse; NumpadInput = SacnConfig.Universe.ToString(); });
            SelectArtNetUniverseTargetCommand = new RelayCommand(_ => { ActiveNumpadTarget = NumpadTarget.ArtNetUniverse; NumpadInput = ArtNetConfig.Universe.ToString(); });

            NumpadButtons = BuildNumpad();
        }

        private ObservableCollection<ChannelDigitButton> BuildNumpad()
        {
            return new ObservableCollection<ChannelDigitButton>
            {
                new ChannelDigitButton("1", NumpadDigitCommand, "1"), new ChannelDigitButton("2", NumpadDigitCommand, "2"), new ChannelDigitButton("3", NumpadDigitCommand, "3"),
                new ChannelDigitButton("4", NumpadDigitCommand, "4"), new ChannelDigitButton("5", NumpadDigitCommand, "5"), new ChannelDigitButton("6", NumpadDigitCommand, "6"),
                new ChannelDigitButton("7", NumpadDigitCommand, "7"), new ChannelDigitButton("8", NumpadDigitCommand, "8"), new ChannelDigitButton("9", NumpadDigitCommand, "9"),
                new ChannelDigitButton("Clear", NumpadClearCommand),  new ChannelDigitButton("0", NumpadDigitCommand, "0"), new ChannelDigitButton("Set", NumpadSetCommand)
            };
        }

        private void AddDigit(string? digit)
        {
            if (digit == null) return;
            if (NumpadInput.Length < 4)
                NumpadInput += digit;
        }

        private void AssignNumpadInput()
        {
            if (!int.TryParse(NumpadInput, out int value))
            {
                System.Windows.MessageBox.Show("Ungültige Zahl!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            switch (ActiveNumpadTarget)
            {
                case NumpadTarget.SacnChannel:
                    if (value < 1 || value > 512)
                    {
                        System.Windows.MessageBox.Show("Kanalnummer muss zwischen 1 und 512 liegen.", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (SelectedSacnOutput != null && OutputMappings.Any(x => x != SelectedSacnOutput && x.SacnChannel == value))
                    {
                        System.Windows.MessageBox.Show("Kanal bereits vergeben!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (SelectedSacnOutput != null)
                        SelectedSacnOutput.SacnChannel = value;
                    NumpadInput = "";
                    break;

                case NumpadTarget.ArtNetChannel:
                    if (value < 1 || value > 512)
                    {
                        System.Windows.MessageBox.Show("Kanalnummer muss zwischen 1 und 512 liegen.", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (SelectedArtNetOutput != null && OutputMappings.Any(x => x != SelectedArtNetOutput && x.ArtNetChannel == value))
                    {
                        System.Windows.MessageBox.Show("Kanal bereits vergeben!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (SelectedArtNetOutput != null)
                        SelectedArtNetOutput.ArtNetChannel = value;
                    NumpadInput = "";
                    break;

                case NumpadTarget.SacnUniverse:
                    if (value < 0 || value > 2047)
                    {
                        System.Windows.MessageBox.Show("Universe muss zwischen 0 und 2047 liegen!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (ArtNetConfig.Enabled && value == ArtNetConfig.Universe)
                    {
                        System.Windows.MessageBox.Show("Universe bereits bei artNET vergeben!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    SacnConfig.Universe = value;
                    NumpadInput = "";
                    break;

                case NumpadTarget.ArtNetUniverse:
                    if (value < 0 || value > 2047)
                    {
                        System.Windows.MessageBox.Show("Universe muss zwischen 0 und 2047 liegen!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    if (SacnConfig.Enabled && value == SacnConfig.Universe)
                    {
                        System.Windows.MessageBox.Show("Universe bereits bei sACN vergeben!", "Fehler", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return;
                    }
                    ArtNetConfig.Universe = value;
                    NumpadInput = "";
                    break;

                default:
                    // Kein Target aktiv, keine Aktion
                    break;
            }
        }

        private void ClearSacnChannel(object? parameter)
        {
            if (parameter is DmxOutputMappingViewModel mapping && SacnConfig.Enabled)
                mapping.SacnChannel = null;
        }

        private void ClearArtNetChannel(object? parameter)
        {
            if (parameter is DmxOutputMappingViewModel mapping && ArtNetConfig.Enabled)
                mapping.ArtNetChannel = null;
        }

        private void OnChannelChanged() { }

        private void OnProtocolConfigChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Universum ändern: Kollision verhindern
            if (e.PropertyName == nameof(DmxProtocolConfig.Universe))
            {
                if (SacnConfig.Universe < 0 || SacnConfig.Universe > 2047)
                    SacnConfig.Universe = 10;
                if (ArtNetConfig.Universe < 0 || ArtNetConfig.Universe > 2047)
                    ArtNetConfig.Universe = 11;

                if (SacnConfig.Enabled && ArtNetConfig.Enabled && SacnConfig.Universe == ArtNetConfig.Universe)
                {
                    System.Windows.MessageBox.Show(
                        $"sACN und artNET dürfen nicht dasselbe Universe ({SacnConfig.Universe}) verwenden.",
                        "Ungültige Konfiguration",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                    if (sender == SacnConfig)
                        SacnConfig.Universe = FindNextFreeUniverse(SacnConfig.Universe, ArtNetConfig.Universe);
                    else
                        ArtNetConfig.Universe = FindNextFreeUniverse(ArtNetConfig.Universe, SacnConfig.Universe);
                }
            }

            // Prüfung bei Aktivierung
            if (e.PropertyName == nameof(DmxProtocolConfig.Enabled))
            {
                if (SacnConfig.Enabled && ArtNetConfig.Enabled && SacnConfig.Universe == ArtNetConfig.Universe)
                {
                    if (sender == SacnConfig)
                        SacnConfig.Universe = FindNextFreeUniverse(SacnConfig.Universe, ArtNetConfig.Universe);
                    else
                        ArtNetConfig.Universe = FindNextFreeUniverse(ArtNetConfig.Universe, SacnConfig.Universe);

                    System.Windows.MessageBox.Show(
                        "Universe wurde automatisch geändert, weil es bereits vom anderen Protokoll verwendet wurde.",
                        "Hinweis", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }

        private int FindNextFreeUniverse(int preferred, int blocked)
        {
            int next = preferred + 1;
            if (next > 2047) next = 0;
            if (next == blocked) next = (blocked + 1) % 2048;
            return next;
        }

        private void SaveAndClose()
        {
            if (!SacnConfig.Enabled && !ArtNetConfig.Enabled)
            {
                System.Windows.MessageBox.Show(
                    "Mindestens ein Protokoll (sACN oder artNET) muss aktiviert sein.",
                    "Ungültige Konfiguration",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            var mappingConfig = new DmxMappingConfig
            {
                Sacn = SacnConfig,
                ArtNet = ArtNetConfig,
                Outputs = OutputMappings.Select(vm => vm.ToModel()).ToList()
            };
            _config.Settings.DmxMapping = mappingConfig;
            _config.SaveConfiguration();
            _closeCallback?.Invoke();
        }

        private void ClearAllMappings()
        {
            foreach (var vm in OutputMappings)
            {
                vm.SacnChannel = null;
                vm.ArtNetChannel = null;
            }
        }

        private static DmxMappingConfig CreateDefaultConfig()
        {
            return new DmxMappingConfig
            {
                Sacn = new DmxProtocolConfig { Enabled = false, Universe = 10 },
                ArtNet = new DmxProtocolConfig { Enabled = false, Universe = 11 },
                Outputs = new System.Collections.Generic.List<DmxOutputMapping>
                {
                    new DmxOutputMapping { OutputName = "BPM" },
                    new DmxOutputMapping { OutputName = "S2L-Low" },
                    new DmxOutputMapping { OutputName = "S2L-Low-Middle" },
                    new DmxOutputMapping { OutputName = "S2L-Middle" },
                    new DmxOutputMapping { OutputName = "S2L-High" },
                    new DmxOutputMapping { OutputName = "B2L-Main" },
                    new DmxOutputMapping { OutputName = "B2L-STEM" }
                }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class DmxOutputMappingViewModel : INotifyPropertyChanged
    {
        private readonly DmxOutputMapping _model;
        private readonly ObservableCollection<DmxOutputMappingViewModel> _allMappings;
        private readonly Action _onChannelChanged;

        public DmxOutputMappingViewModel(
            DmxOutputMapping model,
            ObservableCollection<DmxOutputMappingViewModel> allMappings,
            Action onChannelChanged)
        {
            _model = model;
            _allMappings = allMappings;
            _onChannelChanged = onChannelChanged;

            OutputName = model.OutputName;
            SacnChannel = model.SacnChannel;
            ArtNetChannel = model.ArtNetChannel;
        }

        public string OutputName { get; }

        private int? _sacnChannel;
        public int? SacnChannel
        {
            get => _sacnChannel;
            set
            {
                var sanitized = (value is >= 1 and <= 512) ? value : null;
                if (sanitized is >= 1 and <= 512)
                {
                    bool duplicate = _allMappings
                        .Where(x => !ReferenceEquals(x, this))
                        .Any(x => x.SacnChannel == sanitized);
                    if (duplicate)
                        return;
                }
                if (_sacnChannel != sanitized)
                {
                    _sacnChannel = sanitized;
                    _model.SacnChannel = sanitized;
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
                var sanitized = (value is >= 1 and <= 512) ? value : null;
                if (sanitized is >= 1 and <= 512)
                {
                    bool duplicate = _allMappings
                        .Where(x => !ReferenceEquals(x, this))
                        .Any(x => x.ArtNetChannel == sanitized);
                    if (duplicate)
                        return;
                }
                if (_artNetChannel != sanitized)
                {
                    _artNetChannel = sanitized;
                    _model.ArtNetChannel = sanitized;
                    OnPropertyChanged();
                    _onChannelChanged?.Invoke();
                }
            }
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

    // --- Numpad-Button-ViewModel ---
    public class ChannelDigitButton
    {
        public string Label { get; }
        public ICommand Command { get; }
        public string? Digit { get; }

        public ChannelDigitButton(string label, ICommand command, string? digit = null)
        {
            Label = label;
            Command = command;
            Digit = digit;
        }
    }
}
