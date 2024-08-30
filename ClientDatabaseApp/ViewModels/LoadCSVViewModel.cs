using ClientDatabaseApp.Models;
using ClientDatabaseApp.Services;
using ClientDatabaseApp.Services.Exceptions;
using ClientDatabaseApp.Services.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public LoadCSVViewModel(IClientRepo clientRepo, IDialogService dialogService)
        {
            _clientRepo = clientRepo;
            _dialogService = dialogService;
            GetClientsFromCSVCommand = new DelegateCommand<object>(GetClientsFromCSV);
            AddToDatabaseCommand = new DelegateCommand<object>(AddToDatabase);

            PreviewClients = new ObservableCollection<Client>();
        }

        private void GetClientsFromCSV(object e)
        {
            IsLoading = true;
            var filePath = _dialogService.OpenFileDialog("CSV Files (*.csv)|*.csv|All files (*.*)|*.*", "Wybierz plik CSV");
            if (filePath != null)
            {
                try
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
                        PreviewClients = new ObservableCollection<Client>(records);
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"Loading CSV file problem: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }
            else
            {
                IsLoading = false;
            }
        }


        private async void AddToDatabase(object e)
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
