using ClientDatabaseApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.Views
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
