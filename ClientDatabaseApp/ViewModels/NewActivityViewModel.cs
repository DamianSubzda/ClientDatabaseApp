using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ClientDatabaseApp.ViewModel
{
    public class NewActivityViewModel : INotifyPropertyChanged
    {
        private readonly Action _closeAction;

        public ICommand AddActivityCommand { get; set; }
        public Client Client { get; set; }
        public DateTime SelectedDate { get; set; }
        public string Note { get; set; }
        private string _richTextContent;

        public event PropertyChangedEventHandler PropertyChanged;

        public string RichTextContent
        {
            get => _richTextContent;
            set
            {
                _richTextContent = value;
                OnPropertyChanged(nameof(RichTextContent));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;
        public NewActivityViewModel(Client client, Action closeAction, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _activityRepo = activityRepo;
            _dialogService = dialogService;
            SelectedDate = DateTime.Now;
            this.Client = client;
            this._closeAction = closeAction;
            AddActivityCommand = new DelegateCommand<RoutedEventArgs>(AddActivity);
        }

        private void AddActivity(RoutedEventArgs e)
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
