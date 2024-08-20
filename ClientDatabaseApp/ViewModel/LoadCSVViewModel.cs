using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class LoadCSVViewModel : INotifyPropertyChanged
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
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LoadCSVViewModel()
        {
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


        private void AddToDatabase(RoutedEventArgs e)//TODO
        {
            IsLoading = true;

            //await Task.Run(() =>
            //{
            //    DatabaseQuery query = new DatabaseQuery();
            //    List<(string, string)> result = query.TryAddClients(PreviewClients);

            //    App.Current.Dispatcher.Invoke(() =>
            //    {
            //        foreach (var item in result)
            //        {
            //            MessageBox.Show(item.Item2, item.Item1);
            //        }
            //        PreviewClients.Clear();
            //    });
            //});

            IsLoading = false;
        }

    }
}
