using ClientDatabaseApp.ViewModels;
using System.Windows;
using Unity;

namespace ClientDatabaseApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<MainWindowViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
