using ClientDatabaseApp.ViewModel;
using System.Windows.Controls;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy LoadCSV.xaml
    /// </summary>
    public partial class LoadCSV : UserControl
    {
        public LoadCSV()
        {
            DataContext = new MVLoadCSVMainWindow();
            InitializeComponent();
        }
    }
}
