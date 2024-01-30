using ClientDatabaseApp.ViewModel;
using System.Windows.Controls;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy ClientDatabase.xaml
    /// </summary>
    public partial class ClientDatabase : UserControl
    {
        public ClientDatabase()
        {
            DataContext = new MVClientDatabaseMainWindow();
            InitializeComponent();
        }
    }
}
