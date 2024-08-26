using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;

namespace ClientDatabaseApp.ViewModels
{
    internal class ShowClientViewModel : BaseViewModel
    {
        public ICommand EditDataCommand { get; set; }
        public ICommand SaveDataCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        private Client _client;
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
        private bool _isEditing;

        private Action _closeAction;

        private StatusItem _selectedStatus;
        private ObservableCollection<StatusItem> _statusItems;

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
                DateTextBox = value.Data ?? DateTime.Now;
                OwnerTextBox = value.Owner;
                RichTextContent = value.Note;
                SelectedStatus = StatusItems[value.Status];
            }
        }

        public string ClientNameTextBox
        {
            get => _clientNameTextBox;
            set => SetField(ref _clientNameTextBox, value, nameof(ClientNameTextBox));
        }

        public string PhonenumberTextBox
        {
            get => _phonenumberTextBox;
            set => SetField(ref _phonenumberTextBox, value, nameof(PhonenumberTextBox));
        }

        public string EmailTextBox
        {
            get => _emailTextBox;
            set => SetField(ref _emailTextBox, value, nameof(EmailTextBox));
        }

        public string CityTextBox
        {
            get => _cityTextBox;
            set => SetField(ref _cityTextBox, value, nameof(CityTextBox));
        }

        public string FacebookTextBox
        {
            get => _facebookTextBox;
            set => SetField(ref _facebookTextBox, value, nameof(FacebookTextBox));
        }

        public string InstagramTextBox
        {
            get => _instagramTextBox;
            set => SetField(ref _instagramTextBox, value, nameof(InstagramTextBox));
        }

        public string PageURLTextBox
        {
            get => _pageURLTextBox;
            set => SetField(ref _pageURLTextBox, value, nameof(PageURLTextBox));
        }

        public DateTime DateTextBox
        {
            get => _dateTextBox;
            set => SetField(ref _dateTextBox, value, nameof(DateTextBox));
        }

        public string OwnerTextBox
        {
            get => _ownerTextBox;
            set => SetField(ref _ownerTextBox, value, nameof(OwnerTextBox));
        }

        public string RichTextContent
        {
            get => _richTextContent;
            set => SetField(ref _richTextContent, value, nameof(RichTextContent));
        }

        public ObservableCollection<StatusItem> StatusItems
        {
            get => _statusItems;
            set => SetField(ref _statusItems, value, nameof(StatusItems));
        }

        public StatusItem SelectedStatus
        {
            get => _selectedStatus;
            set => SetField(ref _selectedStatus, value, nameof(SelectedStatus));
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetField(ref _isEditing, value, nameof(IsEditing));
        }


        private IClientRepo _clientRepo;
        private IDialogService _dialogService;
        public ShowClientViewModel(Client client, Action closeAction, IClientRepo clientRepo, IDialogService dialogService)
        {
            ComboboxStatus combobox = new ComboboxStatus();
            StatusItems = combobox.StatusItems;

            Client = client;
            _closeAction = closeAction;
            _clientRepo = clientRepo;
            _dialogService = dialogService;

            EditDataCommand = new DelegateCommand<RoutedEventArgs>(EditData);
            SaveDataCommand = new DelegateCommand<RichTextBox>(SaveData);
            ExitCommand = new DelegateCommand<RoutedEventArgs>(ExitWindow);

            IsEditing = false;
        }

        private void EditData(RoutedEventArgs e)
        {
            IsEditing = true;
        }

        private void SaveData(RichTextBox richTextBox)
        {
            string note = RichTextBoxHelper.GetTextFromRichTextBox(richTextBox);
            var client = new Client
            {
                ClientId = Client.ClientId,
                ClientName = ClientNameTextBox,
                Phonenumber = PhonenumberTextBox,
                Email = EmailTextBox,
                City = CityTextBox,
                Facebook = FacebookTextBox,
                Instagram = InstagramTextBox,
                PageURL = PageURLTextBox,
                Data = DateTextBox,
                Owner = OwnerTextBox,
                Note = note,
                Status = (int)SelectedStatus.Value
            };

            try
            {
                _clientRepo.UpdateClient(client);
            }
            catch
            {
                _dialogService.ShowMessage("Wystąpił błąd podczas aktualizowania informacji o kliencie! \nSprawdź wprowadzone dane!");
            }
            

            IsEditing = false;
        }

        private void ExitWindow(RoutedEventArgs e)
        {
            _closeAction?.Invoke();
        }

    }
}
