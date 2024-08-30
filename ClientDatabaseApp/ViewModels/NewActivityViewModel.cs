using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModels
{
    public class NewActivityViewModel : BaseViewModel, IBaseDialog
    {
        public ICommand AddActivityCommand { get; set; }
        
        public Client Client { get; set; }
        public DateTime SelectedDate { get; set; }
        public Action CloseAction { get; set; }

        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;
        public NewActivityViewModel(Client client, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _activityRepo = activityRepo;
            _dialogService = dialogService;
            SelectedDate = DateTime.Now;
            Client = client;
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

            try
            {
                _activityRepo.CreateActivity(Client, SelectedDate, note);
                ExitWindow(null);
            }
            catch
            {
                _dialogService.ShowMessage("Wystąpił błąd podczas próby stworzenia nowego wydarzenia! \nSprawdź wprowadzone dane!");
            }
            
            
        }
        public void ExitWindow(object e)
        {
            CloseAction?.Invoke();
        }
    }
}
