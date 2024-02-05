using ClientDatabaseApp.ViewModel;
using System.Windows.Controls;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy AddClient.xaml
    /// </summary>
    public partial class AddClient : UserControl
    {
        public AddClient()
        {
            DataContext = new AddClientViewModel();
            InitializeComponent();
        }
    }
}
