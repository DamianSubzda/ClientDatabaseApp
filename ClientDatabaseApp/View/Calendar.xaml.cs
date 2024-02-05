using ClientDatabaseApp.ViewModel;
using System.Windows.Controls;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        public Calendar()
        {
            DataContext = new CalendarViewModel();
            InitializeComponent();
        }
    }
}
