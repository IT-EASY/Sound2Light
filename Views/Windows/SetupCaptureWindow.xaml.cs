using Microsoft.Extensions.DependencyInjection;

using Sound2Light.Config;

using System.Diagnostics;
using System.Windows;

namespace Sound2Light.Views.Windows
{
    public partial class SetupCaptureWindow : Window
    {
        public SetupCaptureWindow()
        {
            InitializeComponent();
            Debug.WriteLine("[DEBUG] SetupCaptureWindow wurde geöffnet!");
        }
    }
}
