using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;

namespace ClientDatabaseApp.ViewModels
{
    public class AddClientViewModel : BaseViewModel
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
                Status = (int)SelectedStatus.Value
            };

            try
            {
                await _clientRepo.AddClient(client);
            }
            catch
            {
                _dialogService.ShowMessage($"Wystąpił błąd podczas próby dodania klienta: {client.ClientName}");
            }
            finally
            {
                ClientNameTextBox = string.Empty;
                PhonenumberTextBox = string.Empty;
                EmailTextBox = string.Empty;
                CityTextBox = string.Empty;
                FacebookTextBox = string.Empty;
                InstagramTextBox = string.Empty;
                PageURLTextBox = string.Empty;
                DateTextBox = DateTime.Now;
                OwnerTextBox = string.Empty;
                RichTextContent = string.Empty;
                SelectedStatus = StatusItems[0];
            }

        }



    }
}
