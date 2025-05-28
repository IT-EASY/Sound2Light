using Sound2Light.ViewModels.Units;

namespace Sound2Light.ViewModels.Main
{
    public class MainViewModel
    {
        public PowerButtonViewModel PowerButton { get; }
        public UnitSetupViewModel UnitSetup { get; }

        public MainViewModel(PowerButtonViewModel powerButtonViewModel, 
                             UnitSetupViewModel unitSetupViewModel)
        {
            PowerButton = powerButtonViewModel;
            UnitSetup = unitSetupViewModel;
        }
    }
}
