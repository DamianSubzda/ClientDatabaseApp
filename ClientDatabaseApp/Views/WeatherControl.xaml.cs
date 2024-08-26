using ClientDatabaseApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.Views
{
    /// <summary>
    /// Interaction logic for WeatherControl.xaml
    /// </summary>
    public partial class WeatherControl : UserControl
    {
        public WeatherControl()
        {
            InitializeComponent();
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<WeatherViewModel>();
            DataContext = viewModel;
        }
    }
}
