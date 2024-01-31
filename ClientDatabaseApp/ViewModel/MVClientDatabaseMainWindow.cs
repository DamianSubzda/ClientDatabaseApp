using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class MVClientDatabaseMainWindow : INotifyPropertyChanged
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

        public MVClientDatabaseMainWindow()
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

        }

        private void RemoveSelected(RoutedEventArgs e)
        {

        }

        private void AddFolowUp(RoutedEventArgs e)
        {

        }


        void AddFollowUp()
        {
            try
            {
                conn.Open();

                string insertQuery = "INSERT INTO `FollowUp` " +
                    "(`ClientId`, `Note`, `DateOfCreation`, `DateOfAction`) " +
                    "VALUES " +
                    "(@ClientId, @Note, @DateOfCreation, @DateOfAction)";

                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@ClientId", 1);
                cmd.Parameters.AddWithValue("@Note", "Notatka dla klienta");
                cmd.Parameters.AddWithValue("@DateOfCreation", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateOfAction", DateTime.Now.AddDays(3));

                cmd.ExecuteNonQuery();

                Console.WriteLine("Insert wykonany pomyślnie.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


    }
}

