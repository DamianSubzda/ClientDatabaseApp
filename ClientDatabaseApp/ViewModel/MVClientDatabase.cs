using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.View;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class MVClientDatabase : INotifyPropertyChanged
    {
        private MySqlConnection conn = DatabaseConnector.connection;

        public ICommand ShowMoreDetailsCommand { get; private set; }
        public ICommand RemoveSelectedCommand { get; private set; }
        public ICommand AddFolowUpCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Client> _client;
        public ObservableCollection<Client> Client
        {
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
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


        public MVClientDatabase()
        {
            ShowMoreDetailsCommand = new DelegateCommand<RoutedEventArgs>(ShowMoreDetails);
            RemoveSelectedCommand = new DelegateCommand<RoutedEventArgs>(RemoveSelected);
            AddFolowUpCommand = new DelegateCommand<RoutedEventArgs>(AddFolowUp);
            InitializeClientsGrid();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void InitializeClientsGrid()
        {
            var context = new DBContextHVAC();
            var clients = context.ClientDBSet.ToList();

            ObservableCollection<Client> _clientsTemp = new ObservableCollection<Client>(clients);
            Client = _clientsTemp;
        }

        private void ShowMoreDetails(RoutedEventArgs e)
        {
            DatabaseQuery query = new DatabaseQuery();
        }

        private void RemoveSelected(RoutedEventArgs e)
        {
            if (SelectedClient != null)
            {
                MessageBoxResult result = MessageBox.Show($"Jesteś pewny usunięcia klienta: {SelectedClient.ClientName}?", "Uwaga", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    //Usuwanie
                    DatabaseQuery query = new DatabaseQuery();
                    string exception = query.DeleteClient(SelectedClient);
                    if (!string.IsNullOrEmpty(exception))
                    {
                        _ = MessageBox.Show(exception);
                    }
                    else
                    {
                        Client.Remove(SelectedClient);
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
                MVNewFollowUp newFollowUpViewModel = new MVNewFollowUp(SelectedClient, () => newFollowUp.Close());
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

