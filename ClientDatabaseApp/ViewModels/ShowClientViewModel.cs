using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using static ClientDatabaseApp.Service.ComboboxStatus;

namespace ClientDatabaseApp.ViewModel
{
    public class ShowClientViewModel : INotifyPropertyChanged
    {
        
        public ICommand EditDataCommand { get; set; }
        public ICommand SaveDataCommand { get; set; }
        public ICommand ExitCommand { get; set; }


        private Client _client;
        private Action _closeAction;
        private string _clientNameTextBox;
        private string _phonenumberTextBox;
        private string _emailTextBox;
        private string _cityTextBox;
        private string _facebookTextBox;
        private string _instagramTextBox;
        private string _pageURLTextBox;
        private DateTime _dateTextBox;
        private string _ownerTextBox;
        private string _richTextContent;
        private StatusItem _selectedStatus;

        private ObservableCollection<StatusItem> _statusItems;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ClientNameTextBox
        {
            get => _clientNameTextBox;
            set
            {
                if (value != null)
                {
                    _clientNameTextBox = value;
                    OnPropertyChanged(nameof(ClientNameTextBox));
                }
            }
        }
        public string PhonenumberTextBox
        {
            get => _phonenumberTextBox;
            set
            {
                if (value != null)
                {
                    _phonenumberTextBox = value;
                    OnPropertyChanged(nameof(PhonenumberTextBox));
                }
            }
        }
        public string EmailTextBox
        {
            get => _emailTextBox;
            set
            {
                if (value != null)
                {
                    _emailTextBox = value;
                    OnPropertyChanged(nameof(EmailTextBox));
                }
            }
        }
        public string CityTextBox
        {
            get => _cityTextBox;
            set
            {
                if (value != null)
                {
                    _cityTextBox = value;
                    OnPropertyChanged(nameof(CityTextBox));
                }
            }
        }
        public string FacebookTextBox
        {
            get => _facebookTextBox;
            set
            {
                if (value != null)
                {
                    _facebookTextBox = value;
                    OnPropertyChanged(nameof(FacebookTextBox));
                }
            }
        }
        public string InstagramTextBox
        {
            get => _instagramTextBox;
            set
            {
                if (value != null)
                {
                    _instagramTextBox = value;
                    OnPropertyChanged(nameof(InstagramTextBox));
                }
            }
        }
        public string PageURLTextBox
        {
            get => _pageURLTextBox;
            set
            {
                if (value != null)
                {
                    _pageURLTextBox = value;
                    OnPropertyChanged(nameof(PageURLTextBox));
                }
            }
        }
        public DateTime DateTextBox
        {
            get => _dateTextBox;
            set
            {
                if (value != null)
                {
                    _dateTextBox = value;
                    OnPropertyChanged(nameof(DateTextBox));
                }
            }
        }
        public string OwnerTextBox
        {
            get => _ownerTextBox;
            set
            {
                if (value != null)
                {
                    _ownerTextBox = value;
                    OnPropertyChanged(nameof(OwnerTextBox));
                }
            }
        }
        public string RichTextContent
        {
            get => _richTextContent;
            set
            {
                _richTextContent = value;
                OnPropertyChanged(nameof(RichTextContent));
                
            }
        }
        public ObservableCollection<StatusItem> StatusItems
        {
            get => _statusItems;

            set
            {
                _statusItems = value;
                OnPropertyChanged(nameof(StatusItems));
            }
        }
        public StatusItem SelectedStatus
        {
            get => _selectedStatus;

            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                ClientNameTextBox = value.ClientName;
                PhonenumberTextBox = value.Phonenumber;
                EmailTextBox = value.Email;
                CityTextBox = value.City;
                FacebookTextBox = value.Facebook;
                InstagramTextBox = value.Instagram;
                PageURLTextBox = value.PageURL;
                DateTextBox = (DateTime)value.Data;
                OwnerTextBox = value.Owner;
                RichTextContent = value.Note;
                SelectedStatus = StatusItems[value.Status];
                OnPropertyChanged(nameof(Client));
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ShowClientViewModel(Client client, Action closeAction)
        {
            ComboboxStatus combobox = new ComboboxStatus();
            StatusItems = combobox.StatusItems;

            Client = client;
            IsEditing = false;
            _closeAction = closeAction;         

            EditDataCommand = new DelegateCommand<RoutedEventArgs>(EditData);
            SaveDataCommand = new DelegateCommand<RichTextBox>(SaveData);
            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);

        }

        private void EditData(RoutedEventArgs e)
        {
            IsEditing = true;
        }

        private void SaveData(RichTextBox richTextBox)
        {
            
            string note = RichTextBoxHelper.GetTextFromRichTextBox(richTextBox);
            //TODO
            IsEditing = false;
        }

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }

    }
}
