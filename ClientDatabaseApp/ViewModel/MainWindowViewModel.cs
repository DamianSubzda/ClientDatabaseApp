using ClientDatabaseApp.Service;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class MainWindowViewModel
    {
        public ICommand TabSelectionChangedCommand { get; private set; }
        public MainWindowViewModel()
        {
            TabSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(TabSelectionChanged);
        }


        private void TabSelectionChanged(SelectionChangedEventArgs e)
        {
            
        }
    }
}
