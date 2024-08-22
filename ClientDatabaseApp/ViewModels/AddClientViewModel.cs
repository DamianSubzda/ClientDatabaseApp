using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using static ClientDatabaseApp.Service.ComboboxStatus;

namespace ClientDatabaseApp.ViewModel
{
    public class AddClientViewModel : INotifyPropertyChanged
    {
        public ICommand AddClientToDatabaseCommand { get; set; }
        public ICommand SaveRichTextContentCommand { get; set; }

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


        private readonly IClientRepo _clientRepo;
        private readonly IDialogService _dialogService;
        public AddClientViewModel(IClientRepo clientRepo, IDialogService dialogService)
        {
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            DateTextBox = DateTime.Now;
            AddClientToDatabaseCommand = new DelegateCommand<RichTextBox>(AddClientAsync);
            SaveRichTextContentCommand = new DelegateCommand<RichTextBox>(SaveRichTextContent);
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

        public void SaveRichTextContent(RichTextBox richTextBox)
        {
            RichTextContent = RichTextBoxHelper.GetTextFromRichTextBox(richTextBox);
        }


        private async void AddClientAsync(RichTextBox richTextBox)
        {

            SaveRichTextContent(richTextBox);

            if (string.IsNullOrEmpty(ClientNameTextBox))
            {
                _dialogService.ShowMessage("Brak nazwy klienta!");
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
                Note = RichTextContent,
                Status = (int)SelectedStatus.Value //Może podmienić i zamiast numerka będzie wartość z 'Enuma'
            };

            try
            {
                await _clientRepo.AddClient(client);
                //Clear all fields
            }
            catch
            {
                _dialogService.ShowMessage($"Wystąpił błąd podczas próby dodania klienta: {client.ClientName}");
            }
            
        }



    }
}
