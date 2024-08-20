using ClientDatabaseApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy AddClient.xaml
    /// </summary>
    public partial class AddClient : UserControl
    {
        public AddClient()
        {
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<AddClientViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
