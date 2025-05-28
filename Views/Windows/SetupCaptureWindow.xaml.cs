using Microsoft.Extensions.DependencyInjection;
using System.Windows;

using Sound2Light.Config;
using Sound2Light.ViewModels.Windows;

namespace Sound2Light.Views.Windows
{
    public partial class SetupCaptureWindow : Window
    {
        public SetupCaptureWindow()
        {
            InitializeComponent();
            DataContext = new SetupCaptureViewModel(
                ((App)Application.Current).Services.GetRequiredService<IAppConfigurationService>(),
                () => this.Close());
        }
    }
}
