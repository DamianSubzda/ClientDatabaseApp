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
    public class ShowFollowUpViewModel : INotifyPropertyChanged
    {
        public ICommand ExitCommand { get; set; }
        private Activity _followup;
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

        public ShowFollowUpViewModel(Activity followup, Action closeAction)
        {
            FollowUp = followup;
            OriginalNote = FollowUp.Note;
            EditableNote = FollowUp.Note;
            DateOfCreation = FollowUp.DateOfCreation;
            DateOfAction = FollowUp.DateOfAction;
            ClientName = FollowUp.Client.ClientName; //Nwm czy zadziała
            _closeAction = closeAction;
            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);
        }

        public Activity FollowUp
        {
            get => _followup;
            set
            {
                _followup = value;
                OnPropertyChanged(nameof(Client));
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
