using ClientDatabaseApp.ViewModel;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        public Calendar()
        {
            var app = (App)Application.Current;
            var viewModel = app.Container.Resolve<CalendarViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
