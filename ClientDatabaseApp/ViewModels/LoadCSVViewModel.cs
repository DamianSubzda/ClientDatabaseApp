using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using ClientDatabaseApp.ViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System.Collections.ObjectModel;
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
