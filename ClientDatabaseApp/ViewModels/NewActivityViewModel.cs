using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using ClientDatabaseApp.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ClientDatabaseApp.ViewModel
{
    public class NewActivityViewModel : BaseViewModel
    {
        public ICommand AddActivityCommand { get; set; }

        private readonly Action _closeAction;
        
        public Client Client { get; set; }
        public DateTime SelectedDate { get; set; }

        private string _richTextContent;

        public string RichTextContent
        {
            get => _richTextContent;
            set => SetField(ref _richTextContent, value, nameof(RichTextContent));
        }

        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;
        public NewActivityViewModel(Client client, Action closeAction, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _activityRepo = activityRepo;
            _dialogService = dialogService;
            SelectedDate = DateTime.Now;
            Client = client;
            _closeAction = closeAction;
            AddActivityCommand = new DelegateCommand<RoutedEventArgs>(AddActivity);
        }

        private void AddActivity(RoutedEventArgs e)//Możliwe że wypadałoby skorzystać z helpera!
        {
            if (string.IsNullOrEmpty(RichTextContent))
            {
                _dialogService.ShowMessage("Brak podanej notatki!");
                return;
            }
            XDocument document = XDocument.Parse(RichTextContent);
            XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            var activityNote = string.Join(" ", document.Descendants(ns + "Run")
                                           .Select(run => run.Value));

            _activityRepo.CreateActivity(Client, SelectedDate, activityNote);
            _closeAction?.Invoke();
        }

    }
}
