using System.Windows.Controls;

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
