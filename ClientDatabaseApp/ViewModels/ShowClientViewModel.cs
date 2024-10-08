﻿using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;

namespace ClientDatabaseApp.ViewModels
{
    internal class ShowClientViewModel : BaseViewModel, IBaseDialog
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

        private StatusItem _selectedStatus;
        private ObservableCollection<StatusItem> _statusItems;

        public Action CloseAction { get; set; }

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
                SelectedStatus = _comboboxStatus.GetStatusItems()[value.Status];
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


        private readonly IClientRepo _clientRepo;
        private readonly IDialogService _dialogService;
        private readonly IComboboxStatus _comboboxStatus;
        public ShowClientViewModel(Client client, IClientRepo clientRepo, IDialogService dialogService, IComboboxStatus comboboxStatus)
        {
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            _comboboxStatus = comboboxStatus;

            StatusItems = _comboboxStatus.GetStatusItems() ?? new ObservableCollection<StatusItem>();

            EditDataCommand = new DelegateCommand<object>(EditData);
            SaveDataCommand = new DelegateCommand<RichTextBox>(SaveData);
            ExitCommand = new DelegateCommand<object>(ExitWindow);

            Client = client;

            IsEditing = false;
        }

        private void EditData(object e)
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

        public void ExitWindow(object e)
        {
            CloseAction?.Invoke();
        }
    }
}
