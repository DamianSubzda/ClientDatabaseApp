using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class ShowActivityViewModel : INotifyPropertyChanged
    {
        public ICommand ExitCommand { get; set; }
        private Activity _activity;
        private Action _closeAction;
        private string _originalNote;
        private string _editableNote;
        private DateTime _dateOfCreation;
        private DateTime _dateOfAction;
        private string _clientName;

        public string OriginalNote
        {
            get => _originalNote;
            set
            {
                _originalNote = value;
                OnPropertyChanged(nameof(OriginalNote));
            }
        }
        public string EditableNote
        {
            get => _editableNote;
            set
            {
                _editableNote = value;
                OnPropertyChanged(nameof(EditableNote));
            }
        }
        public DateTime DateOfCreation
        {
            get => _dateOfCreation;
            set
            {
                _dateOfCreation = value;
                OnPropertyChanged(nameof(DateOfCreation));
            }
        }
        public DateTime? DateOfAction
        {
            get => _dateOfAction;
            set
            {
                _dateOfAction = (DateTime)value;
                OnPropertyChanged(nameof(DateOfAction));
            }
        }
        public string ClientName
        {
            get => _clientName;
            set
            {
                _clientName = value;
                OnPropertyChanged(nameof(ClientName));
            }
        }

        public Activity Activity
        {
            get => _activity;
            set
            {
                _activity = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        private IClientRepo _clientRepo;
        public ShowActivityViewModel(Activity activity, Action closeAction, IClientRepo clientRepo)
        {
            _clientRepo = clientRepo;
            Activity = activity;
            OriginalNote = Activity.Note;
            EditableNote = Activity.Note;
            DateOfCreation = Activity.DateOfCreation;
            DateOfAction = Activity.DateOfAction;
            _closeAction = closeAction;
            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);

            LoadClientNameAsync(activity.ClientId);
        }

        private async void LoadClientNameAsync(int clientId)
        {
            var client = await _clientRepo.GetClient(clientId);
            if (client != null)
            {
                ClientName = client.ClientName;
            }
            else
            {
                ClientName = "Unknown Client";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }
    }
}
