using Sound2Light.ViewModels.Units;
using Sound2Light.Views.Controls.Units;

namespace Sound2Light.ViewModels.Main
{
    public class MainViewModel
    {
        public PowerButtonViewModel PowerButton { get; }
        public UnitSetupViewModel UnitSetup { get; }
        public UnitCaptureViewModel UnitCapture { get; }

        public MainViewModel(PowerButtonViewModel powerButtonViewModel, 
                             UnitSetupViewModel unitSetupViewModel,
                             UnitCaptureViewModel unitCaptureViewModel)
        {
            PowerButton = powerButtonViewModel;
            UnitSetup = unitSetupViewModel;
            UnitCapture = unitCaptureViewModel;
        }
    }
}
