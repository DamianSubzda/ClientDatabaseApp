using ClientDatabaseApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoadCSV.xaml
    /// </summary>
    public partial class LoadCSV : UserControl
    {
        public LoadCSV()
        {
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<LoadCSVViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
