using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModels
{
    public class ShowActivityViewModel : BaseViewModel
    {
        public ICommand ExitCommand { get; set; }
        public ICommand EditCommand { get; set; }

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
        private IActivityRepo _activityRepo;
        private IDialogService _dialogService;
        public ShowActivityViewModel(Activity activity, Action closeAction, IClientRepo clientRepo, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _clientRepo = clientRepo;
            _activityRepo = activityRepo;
            _dialogService = dialogService;

            Activity = activity;
            OriginalNote = Activity.Note;
            EditableNote = Activity.Note;
            DateOfCreation = Activity.DateOfCreation;
            DateOfAction = Activity.DateOfAction ?? DateTime.MinValue;
            _closeAction = closeAction;

            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);
            EditCommand = new DelegateCommand<RichTextBox>(EditActivity);

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

        private async void EditActivity(RichTextBox richTextBox)
        {
            string note = RichTextBoxHelper.GetTextFromRichTextBox(richTextBox);
            Activity.Note = note;

            try
            {
                await _activityRepo.UpdateActivity(Activity);
                OriginalNote = note;
            }
            catch
            {
                _dialogService.ShowMessage("Wystąpił błąd podczas aktualizowania informacji o wydarzeniu! \nSprawdź wprowadzone dane!");
            }
            
        }

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }
    }
}
