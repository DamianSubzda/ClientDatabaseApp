using ClientDatabaseApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy ClientDatabase.xaml
    /// </summary>
    public partial class ClientDatabase : UserControl
    {
        public ClientDatabase()
        {
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<ClientDatabaseViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
