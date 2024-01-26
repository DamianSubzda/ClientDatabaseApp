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

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy ToDo.xaml
    /// </summary>
    public partial class ToDo : UserControl
    {
        public ToDo()
        {
            //Prawdopodobnie trzeba zrobić DataContext handlera ustawiającego DataContext w zależności od ItemTab
            InitializeComponent();
        }
    }
}
