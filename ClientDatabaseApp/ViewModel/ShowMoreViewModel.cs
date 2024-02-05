using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class ShowMoreViewModel : INotifyPropertyChanged
    {
        public ICommand ExitCommand { get; set; }

        private Client _client;
        private Action _closeAction;

        public event PropertyChangedEventHandler PropertyChanged;

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ShowMoreViewModel(Client client, Action closeAction)
        {
            Client = client;
            _closeAction = closeAction;
            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);
        }

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }

    }
}
