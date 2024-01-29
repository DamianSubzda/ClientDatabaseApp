using MySqlConnector;
using System.Windows;
using ClientDatabaseApp.ViewModel;
using ClientDatabaseApp.Service;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            _ = DatabaseConnector.GetInstance();
            DataContext = new MVMainWindow();
            InitializeComponent();
        }


    }
}
