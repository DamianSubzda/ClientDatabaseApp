using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using ClientDatabaseApp.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class ShowActivityViewModel : BaseViewModel
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
            set => SetField(ref _originalNote, value, nameof(OriginalNote));
        }
        public string EditableNote
        {
            get => _editableNote;
            set=> SetField(ref _editableNote, value, nameof(EditableNote));
        }
        public DateTime DateOfCreation
        {
            get => _dateOfCreation;
            set=> SetField(ref _dateOfCreation, value, nameof(DateOfCreation));
        }
        public DateTime DateOfAction
        {
            get => _dateOfAction;
            set => SetField(ref _dateOfAction, value, nameof(DateOfAction));
        }
        public string ClientName
        {
            get => _clientName;
            set=> SetField(ref _clientName, value, nameof(ClientName));
        }

        public Activity Activity
        {
            get => _activity;
            set=> SetField(ref _activity, value, nameof(Activity));
        }

        private IClientRepo _clientRepo;
        public ShowActivityViewModel(Activity activity, Action closeAction, IClientRepo clientRepo)
        {
            _clientRepo = clientRepo;
            Activity = activity;
            OriginalNote = Activity.Note;
            EditableNote = Activity.Note;
            DateOfCreation = Activity.DateOfCreation;
            DateOfAction = Activity.DateOfAction ?? DateTime.MinValue;
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

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }
    }
}
