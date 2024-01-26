using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientDatabaseApp.ViewModel;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy AddClient.xaml
    /// </summary>
    public partial class AddClient : UserControl
    {
        public AddClient()
        {
            DataContext = new MVAddClientMainWindow();
            InitializeComponent();
        }
    }
}
