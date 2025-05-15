using Sound2Light.Views.Controls.Units;
using Sound2Light.Views.Controls.Visual;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sound2Light
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // MainWindow.xaml.cs
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}