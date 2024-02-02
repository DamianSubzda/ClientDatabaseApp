using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ClientDatabaseApp.ViewModel
{
    public class MVNewFollowUp : INotifyPropertyChanged
    {
        private readonly Action _closeAction;

        public ICommand addFollowUpCommand { get; set; }
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

        public MVNewFollowUp(Client client, Action closeAction)
        {
            SelectedDate = DateTime.Now;
            this.Client = client;
            this._closeAction = closeAction;
            addFollowUpCommand = new DelegateCommand<RoutedEventArgs>(AddFollowUp);
        }

        private void AddFollowUp(RoutedEventArgs e)
        {
            XDocument document = XDocument.Parse(RichTextContent);
            XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            var followUpNote = string.Join(" ", document.Descendants(ns + "Run")
                                           .Select(run => run.Value));
            if (!string.IsNullOrEmpty(followUpNote))
            {
                DatabaseQuery query = new DatabaseQuery();
                query.AddFollowUp(Client, SelectedDate, followUpNote);
            }
            _closeAction?.Invoke();
        }

    }
}
