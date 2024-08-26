using ClientDatabaseApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace ClientDatabaseApp.Views
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
