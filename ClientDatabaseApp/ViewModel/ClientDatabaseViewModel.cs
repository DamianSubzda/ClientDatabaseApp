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
        public ICommand AddFolowUpCommand { get; private set; }
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

        public ClientDatabaseViewModel(IClientRepo clientRepo, IActivityRepo activityRepo)
        {
            _clientRepo = clientRepo;
            _activityRepo = activityRepo;
            ShowMoreDetailsCommand = new DelegateCommand<RoutedEventArgs>(ShowMoreDetails);
            RemoveSelectedCommand = new DelegateCommand<RoutedEventArgs>(RemoveSelected);
            AddFolowUpCommand = new DelegateCommand<RoutedEventArgs>(AddFolowUp);
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
            var context = new PostgresContext();
            var clients = await context.Clients.ToListAsync();

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
                MessageBox.Show("Proszę zaznaczyć klienta w tabeli.");
            }
        }

        private void RemoveSelected(RoutedEventArgs e)
        {
            if (SelectedClient != null)
            {
                MessageBoxResult result = MessageBox.Show($"Jesteś pewny usunięcia klienta: {SelectedClient.ClientName}?", "Uwaga", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _clientRepo.DeleteClient(SelectedClient);
                        _clients.Remove(SelectedClient);
                    }
                    catch
                    {
                        //TODO Exceptions
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Brak zaznaczonego klienta w tabeli.");
            }
        }

        private void AddFolowUp(RoutedEventArgs e)
        {

            if (SelectedClient != null)
            {
                NewFollowUp newFollowUp = new NewFollowUp();
                NewFollowUpViewModel newFollowUpViewModel = new NewFollowUpViewModel(SelectedClient, () => newFollowUp.Close(), _activityRepo);
                newFollowUp.DataContext = newFollowUpViewModel;
                newFollowUp.ShowDialog();

            }
            else
            {
                MessageBox.Show("Proszę zaznaczyć klienta w tabeli.");
            }
        }
    }
}

