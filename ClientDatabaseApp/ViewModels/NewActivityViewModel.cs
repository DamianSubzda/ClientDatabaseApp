using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModels
{
    public class NewActivityViewModel : BaseViewModel
    {
        public ICommand AddActivityCommand { get; set; }

        private readonly Action _closeAction;
        
        public Client Client { get; set; }
        public DateTime SelectedDate { get; set; }

        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;
        public NewActivityViewModel(Client client, Action closeAction, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _activityRepo = activityRepo;
            _dialogService = dialogService;
            SelectedDate = DateTime.Now;
            Client = client;
            _closeAction = closeAction;
            AddActivityCommand = new DelegateCommand<RichTextBox>(AddActivity);
        }

        private void AddActivity(RichTextBox richTextBox)
        {
            string note = RichTextBoxHelper.GetTextFromRichTextBox(richTextBox);
            if (string.IsNullOrWhiteSpace(note))
            {
                _dialogService.ShowMessage("Brak podanej notatki!");
                return;
            }

            _activityRepo.CreateActivity(Client, SelectedDate, note);
            _closeAction?.Invoke();
        }


    }
}
