using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Utilities;
using ClientDatabaseApp.Services.Repositories;
using ClientDatabaseApp.Services.Events;
using ClientDatabaseApp.Views;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using static ClientDatabaseApp.Services.Utilities.ComboboxStatus;
using System.Threading.Tasks;

namespace ClientDatabaseApp.ViewModels
{
    public class ClientDatabaseViewModel : BaseViewModel
    {

        public ICommand ShowMoreDetailsCommand { get; private set; }
        public ICommand RemoveSelectedCommand { get; private set; }
        public ICommand AddActivityCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }

        public string FilterText { get; set; }
        private ObservableCollection<Client> _clients;
        private ICollectionView _clientsView;
        private StatusItem _selectedStatus;
        private ObservableCollection<StatusItem> _statusItems;
        private Client _selectedClient;

        public ICollectionView ClientsView
        {
            get => _clientsView;
            set => SetField(ref _clientsView, value, nameof(ClientsView));
        }
        public Client SelectedClient
        {
            get => _selectedClient;
            set => SetField(ref _selectedClient, value, nameof(SelectedClient));
        }
        public ObservableCollection<StatusItem> StatusItems
        {
            get => _statusItems;
            set => SetField(ref _statusItems, value, nameof(StatusItems));
        }
        public StatusItem SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                SetField(ref _selectedStatus, value, nameof(SelectedStatus));
                HandleStatusChange();
            }
        }


        private readonly IClientRepo _clientRepo;
        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IComboboxStatus _comboboxStatus;

        public ClientDatabaseViewModel(IClientRepo clientRepo, IActivityRepo activityRepo, IDialogService dialogService, IEventAggregator eventAggregator, IComboboxStatus comboboxStatus)
        {
            _clientRepo = clientRepo;
            _activityRepo = activityRepo;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _comboboxStatus = comboboxStatus;

            _eventAggregator.GetEvent<ClientAddedToDatabaseEvent>().Subscribe(OnClientAdded);
            _eventAggregator.GetEvent<ClientUpdatedInDatabaseEvent>().Subscribe(OnClientUpdated);
            _eventAggregator.GetEvent<ClientRemovedFromDatabaseEvent>().Subscribe(OnClientRemoved);

            ShowMoreDetailsCommand = new DelegateCommand<object>(ShowMoreDetails);
            RemoveSelectedCommand = new DelegateCommand<object>(RemoveSelected);
            AddActivityCommand = new DelegateCommand<object>(AddActivity);
            FilterCommand = new DelegateCommand<object>(ApplyFilter);

            Initialize();
        }

        private async void Initialize()
        {
            await LoadClientsAsync();
            StatusItems = _comboboxStatus.GetStatusItems();
        }

        private void OnClientAdded(Client newClient)
        {
            _clients.Add(newClient);
            ClientsView.Refresh();
        }

        private void OnClientUpdated(Client updatedClient)
        {
            var clientInList = _clients.FirstOrDefault(c => c.ClientId == updatedClient.ClientId);
            if (clientInList != null)
            {
                clientInList.ClientName = updatedClient.ClientName;
                clientInList.Phonenumber = updatedClient.Phonenumber;
                clientInList.Email = updatedClient.Email;
                clientInList.City = updatedClient.City;
                clientInList.Facebook = updatedClient.Facebook;
                clientInList.Instagram = updatedClient.Instagram;
                clientInList.PageURL = updatedClient.PageURL;
                clientInList.Data = updatedClient.Data;
                clientInList.Owner = updatedClient.Owner;
                clientInList.Note = updatedClient.Note;
                clientInList.Status = updatedClient.Status;

                ClientsView.Refresh();
            }
        }

        private void OnClientRemoved(Client clientToRemove)
        {
            _clients.Remove(clientToRemove);
            ClientsView.Refresh();
        }

        private void HandleStatusChange()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                ClientsView.Filter = (item) =>
                {
                    if (item is Client client)
                    {
                        return client.Status == (int)SelectedStatus.Value;
                    }
                    return false;
                };
            }
            else
            {
                ClientsView.Filter = (item) =>
                {
                    if (item is Client client)
                    {
                        bool isNameMatch = client.ClientName != null && client.ClientName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isCityMatch = client.City != null && client.City.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isDateFilterMatch = false;

                        if (DateTime.TryParseExact(FilterText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var filterDate))
                        {
                            isDateFilterMatch = client.Data == filterDate.Date;
                        }

                        return (isNameMatch || isCityMatch || isDateFilterMatch) && client.Status == (int)SelectedStatus.Value;
                    }
                    return false;
                };
            }

            ClientsView.Refresh();
        }

        private void ApplyFilter(object e)
        {
            SelectedStatus = StatusItems[0];

            if (ClientsView == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(FilterText))
            {
                ClientsView.Filter = null;
            }
            else
            {
                ClientsView.Filter = (item) =>
                {
                    if (item is Client client)
                    {
                        bool isNameMatch = client.ClientName != null && client.ClientName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isCityMatch = client.City != null && client.City.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isDateFilterMatch = false;

                        if (DateTime.TryParseExact(FilterText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var filterDate))
                        {
                            isDateFilterMatch = client.Data == filterDate.Date;
                        }

                        return isNameMatch || isCityMatch || isDateFilterMatch;
                    }
                    return false;
                };
            }

            ClientsView.Refresh();
        }

        internal async Task LoadClientsAsync()
        {
            var clients = await _clientRepo.GetAllClients();

            _clients = new ObservableCollection<Client>(clients);
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
        }

        private void ShowMoreDetails(object e)
        {
            if (SelectedClient != null)
            {
                var viewModel = new ShowClientViewModel(SelectedClient, _clientRepo, _dialogService, _comboboxStatus);
                _dialogService.ShowDialog(viewModel);
            }
            else
            {
                _dialogService.ShowMessage("Proszę zaznaczyć klienta w tabeli.");
            }
        }

        private async void RemoveSelected(object e)
        {
            if (SelectedClient != null)
            {
                bool result = _dialogService.Confirm($"Jesteś pewny usunięcia klienta: {SelectedClient.ClientName}?");
                if (result)
                {
                    try
                    {
                        await _clientRepo.DeleteClient(SelectedClient);
                        _clients.Remove(SelectedClient);
                    }
                    catch
                    {
                        _dialogService.ShowMessage("Wystąpił nieznany błąd podczas próby usunięcia klienta!");
                    }

                }
            }
            else
            {
                _dialogService.ShowMessage("Brak zaznaczonego klienta w tabeli.");
            }
        }

        private void AddActivity(object e)
        {
            if (SelectedClient != null)
            {
                NewActivityViewModel newActivityViewModel = new NewActivityViewModel(SelectedClient, _activityRepo, _dialogService);
                _dialogService.ShowDialog(newActivityViewModel);
            }
            else
            {
                _dialogService.ShowMessage("Proszę zaznaczyć klienta w tabeli.");
            }
        }


    }
}

