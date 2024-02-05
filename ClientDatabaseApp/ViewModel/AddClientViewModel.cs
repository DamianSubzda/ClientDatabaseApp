using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using static ClientDatabaseApp.Service.ComboboxStatus;

namespace ClientDatabaseApp.ViewModel
{
    public class AddClientViewModel : INotifyPropertyChanged //Do zastanowienia - zamiast MessageBox stworzyć serwis dialogowy
    {
        public ICommand AddClientToDatabaseCommand { get; set; }

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
        

        public AddClientViewModel()
        {
            DateTextBox = DateTime.Now;
            AddClientToDatabaseCommand = new DelegateCommand<RoutedEventArgs>(AddClient);
            InitializeComboBoxStatus();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeComboBoxStatus()
        {
            ComboboxStatus combobox = new ComboboxStatus();
            StatusItems = combobox.StatusItems;
            SelectedStatus = StatusItems[0];
        }

        
        private void AddClient(RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ClientNameTextBox))
            {
                _ = MessageBox.Show("Brak nazwy klienta!");
                return;
            }

            Client client = new Client
            {
                ClientName = ClientNameTextBox,
                Phonenumber = PhonenumberTextBox?.Trim(),
                Email = EmailTextBox?.Trim(),
                City = CityTextBox,
                Facebook = FacebookTextBox,
                Instagram = InstagramTextBox,
                PageURL = PageURLTextBox,
                Data = DateTextBox,
                Owner = OwnerTextBox,
                Status = (int)SelectedStatus.Value
            };

            if (string.IsNullOrEmpty(RichTextContent))
            {
                client.Note = "";
            }
            else
            {
                try
                {
                    XDocument document = XDocument.Parse(RichTextContent);
                    XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                    client.Note = string.Join(" ", document.Descendants(ns + "Run")
                                               .Select(run => run.Value));
                }
                catch
                {
                    client.Note = "";
                }

            }

            DatabaseQuery query = new DatabaseQuery();

            var clients = new ObservableCollection<Client> { client };
            List<(string, string)> exceptions = query.TryAddClients(clients);
        }



    }
}
