using ClientDatabaseApp.Service;
using ClientDatabaseApp.ViewModel;
using System.Windows;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DatabaseConnector.GetInstance();
            DataContext = new MVMainWindow();
            InitializeComponent();
        }
    }
}
