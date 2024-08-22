using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.Service.Repository;
using ClientDatabaseApp.View;
using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using static ClientDatabaseApp.Service.ComboboxStatus;

namespace ClientDatabaseApp.ViewModel
{
    public class ClientDatabaseViewModel : INotifyPropertyChanged
    {

        public ICommand ShowMoreDetailsCommand { get; private set; }
        public ICommand RemoveSelectedCommand { get; private set; }
        public ICommand AddActivityCommand { get; private set; }
        public ICommand FilterCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Client> _clients;
        private ICollectionView _clientsView;
        private StatusItem _selectedStatus;
        private ObservableCollection<StatusItem> _statusItems;

        public ICollectionView ClientsView
        {
            get { return _clientsView; }
            set
            {
                _clientsView = value;
                OnPropertyChanged(nameof(ClientsView));
            }
        }
        private Client _selectedClient;
        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
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
                HandleStatusChange();
            }
        }

        public string FilterText { get; set; }
        private readonly IClientRepo _clientRepo;
        private readonly IActivityRepo _activityRepo;
        private readonly IDialogService _dialogService;

        public ClientDatabaseViewModel(IClientRepo clientRepo, IActivityRepo activityRepo, IDialogService dialogService)
        {
            _clientRepo = clientRepo;
            _activityRepo = activityRepo;
            _dialogService = dialogService;

            ShowMoreDetailsCommand = new DelegateCommand<RoutedEventArgs>(ShowMoreDetails);
            RemoveSelectedCommand = new DelegateCommand<RoutedEventArgs>(RemoveSelected);
            AddActivityCommand = new DelegateCommand<RoutedEventArgs>(AddActivity);
            FilterCommand = new DelegateCommand<RoutedEventArgs>(ApplyFilter);
            LoadClients();
            InitializeComboBoxStatus();
            
        }

        private void InitializeComboBoxStatus()
        {
            ComboboxStatus combobox = new ComboboxStatus();
            StatusItems = combobox.StatusItems;
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
                        bool isNameMatch = client.ClientName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isCityMatch = client.City.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
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
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplyFilter(RoutedEventArgs e)
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
                        bool isNameMatch = client.ClientName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                        bool isCityMatch = client.City.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
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

        private async void LoadClients()
        {
            var clients = await _clientRepo.GetAllClients();

            _clients = new ObservableCollection<Client>(clients);
            ClientsView = CollectionViewSource.GetDefaultView(_clients);
        }

        private void ShowMoreDetails(RoutedEventArgs e)
        {
            if (SelectedClient != null)
            {
                ShowClient showMore = new ShowClient();
                ShowClientViewModel showMoreViewModel = new ShowClientViewModel(SelectedClient, () => showMore.Close());
                showMore.DataContext = showMoreViewModel;
                showMore.ShowDialog();

            }
            else
            {
                _dialogService.ShowMessage("Proszę zaznaczyć klienta w tabeli.");
            }
        }

        private void RemoveSelected(RoutedEventArgs e)
        {
            if (SelectedClient != null)
            {
                bool result = _dialogService.Confirm($"Jesteś pewny usunięcia klienta: {SelectedClient.ClientName}?");
                if (result)
                {
                    try
                    {
                        _clientRepo.DeleteClient(SelectedClient);
                        _clients.Remove(SelectedClient);
                    }
                    catch
                    {
                        _dialogService.ShowMessage("Wystąpił błąd podczas usuwania klienta!");
                    }

                }
            }
            else
            {
                _dialogService.ShowMessage("Brak zaznaczonego klienta w tabeli.");
            }
        }

        private void AddActivity(RoutedEventArgs e)
        {

            if (SelectedClient != null)
            {
                NewActivity newActivity = new NewActivity();
                NewActivityViewModel newActivityViewModel = new NewActivityViewModel(SelectedClient, () => newActivity.Close(), _activityRepo, _dialogService);
                newActivity.DataContext = newActivityViewModel;
                newActivity.ShowDialog();

            }
            else
            {
                _dialogService.ShowMessage("Proszę zaznaczyć klienta w tabeli.");
            }
        }
    }
}

