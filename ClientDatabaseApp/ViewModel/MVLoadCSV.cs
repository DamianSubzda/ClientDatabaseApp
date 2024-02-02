using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MVLoadCSV : INotifyPropertyChanged
    {
        public ICommand GetClientsFromCSVCommand { get; set; }
        public ICommand AddToDatabaseCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Client> _previewClients;
        public ObservableCollection<Client> PreviewClients
        {
            get => _previewClients;
            set
            {
                if (_previewClients != value)
                {
                    _previewClients = value;
                    OnPropertyChanged(nameof(PreviewClients));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MVLoadCSV()
        {
            GetClientsFromCSVCommand = new DelegateCommand<RoutedEventArgs>(GetClientsFromCSV);
            AddToDatabaseCommand = new DelegateCommand<RoutedEventArgs>(AddToDatabase);

            PreviewClients = new ObservableCollection<Client>();
        }
        private void GetClientsFromCSV(RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Wybierz plik CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    HeaderValidated = null,
                };

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, config))
                {

                    var records = csv.GetRecords<Client>().ToList();

                    ObservableCollection<Client> _previewClientsTemp = new ObservableCollection<Client>(records);
                    PreviewClients = _previewClientsTemp;
                }
            }
        }

        private void AddToDatabase(RoutedEventArgs e)
        {
            DatabaseQuery query = new DatabaseQuery();
            List<(string, string)> result = query.TryAddClients(PreviewClients);

            foreach (var item in result)
            {
                _ = MessageBox.Show(item.Item2, item.Item1);
            }
            PreviewClients.Clear();
        }
    }
}
