using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Exceptions;
using ClientDatabaseApp.Services.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModels
{
    public class LoadCSVViewModel : BaseViewModel
    {
        public ICommand GetClientsFromCSVCommand { get; set; }
        public ICommand AddToDatabaseCommand { get; set; }

        private ObservableCollection<Client> _previewClients;
        private bool _isLoading;
        public ObservableCollection<Client> PreviewClients
        {
            get => _previewClients;
            set => SetField(ref _previewClients, value, nameof(PreviewClients));
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value, nameof(IsLoading));
        }

        private IClientRepo _clientRepo;
        private IDialogService _dialogService;
        private IEventAggregator _eventAggregator;

        public LoadCSVViewModel(IClientRepo clientRepo, IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            GetClientsFromCSVCommand = new DelegateCommand<RoutedEventArgs>(GetClientsFromCSV);
            AddToDatabaseCommand = new DelegateCommand<RoutedEventArgs>(AddToDatabase);

            PreviewClients = new ObservableCollection<Client>();
        }

        private async void GetClientsFromCSV(RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Wybierz plik CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                IsLoading = true;
                string filePath = openFileDialog.FileName;

                await Task.Run(() =>
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        MissingFieldFound = null,
                        HeaderValidated = null,
                    };

                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, config))
                    {
                        var records = csv.GetRecords<Client>().ToList();
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            PreviewClients = new ObservableCollection<Client>(records);
                        });
                    }
                });

                IsLoading = false;
            }
        }

        private async void AddToDatabase(RoutedEventArgs e)
        {
            IsLoading = true;

            foreach (var client in PreviewClients)
            {
                try
                {
                    await _clientRepo.AddClient(client);
                }
                catch (ClientAlreadyExistsException ex)
                {
                    _dialogService.ShowMessage($"Wystąpił błąd podczas próby zapisu klienta: {ex.Message}");
                }
                catch
                {
                    _dialogService.ShowMessage($"Wystąpił nieoczekiwany błąd przy próbie zapisu: {client.ClientName}");
                }

            }

            PreviewClients.Clear();

            IsLoading = false;
        }


    }
}
