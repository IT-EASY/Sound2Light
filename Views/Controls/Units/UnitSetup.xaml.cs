using System.Windows.Controls;

namespace Sound2Light.Views.Controls.Units
{
    public partial class UnitSetup : UserControl
    {
        public UnitSetup()
        {
            InitializeComponent();
            // 👉 Kein Code-behind-ViewModel-Setup – MVVM-konform über DataContext-Vererbung
        }
    }
}
