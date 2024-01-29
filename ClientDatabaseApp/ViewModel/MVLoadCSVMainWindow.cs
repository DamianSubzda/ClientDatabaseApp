using CsvHelper;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Linq;
using MySqlConnector;
using ClientDatabaseApp.Model;
using ClientDatabaseApp.Service;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class MVLoadCSVMainWindow
    {
        public ICommand GetDataFromCSVCommand { get; set; }
        public ICommand PassDataToDatabaseCoomand { get; set; }

        private MySqlConnection conn = DatabaseConnector.connection;
        public MVLoadCSVMainWindow()
        {
            GetDataFromCSVCommand = new DelegateCommand<RoutedEventArgs>(GetDataFromCSV);
            PassDataToDatabaseCoomand = new DelegateCommand<RoutedEventArgs>(PassDataToDatabase);
        }
        private void GetDataFromCSV(RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Wybierz plik CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    var records = csv.GetRecords<Client>().ToList();
                    //dataGridPreview.ItemsSource = records;
                }
            }
            //dataGridPreview.LayoutUpdated += CheckGridColumnAfterLoad;
        }

        private void PassDataToDatabase(RoutedEventArgs e)
        {
            //int counter = 0;
            //if (conn != null)
            //{
            //    conn.Open();
            //    if (dataGridPreview.ItemsSource is IEnumerable<Client> items)
            //    {
            //        foreach (var item in items)
            //        {
            //            if (!CheckIfClientIsInDatabase(item))
            //            {
            //                if (AddClientToDatabase(item))
            //                {
            //                    counter++;
            //                }
            //            }
            //        }
            //    }
            //    conn.Close();
            //    //MessageBox.Show($"Dodano {counter}/{dataGridPreview.Items.Count}", "Notka");
            //}
            //else
            //{
            //    MessageBox.Show("Brak połączenia z bazą danych");
            //    return;
            //}
        }

        private bool CheckIfClientIsInDatabase(Client record)
        {
            string selectQuery = "SELECT * FROM clients" +
                                  " WHERE FacebookURL = @FacebookURL AND Telefon = @Telefon";
            using (MySqlCommand command = new MySqlCommand(selectQuery, conn))
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FacebookURL", record.Facebook);
                command.Parameters.AddWithValue("@Telefon", record.Phonenumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idclients = reader.GetInt32(reader.GetOrdinal("ClientId"));
                        string klientName = reader.GetString(reader.GetOrdinal("ClientName"));
                        string facebookURL = reader.GetString(reader.GetOrdinal("Facebook"));
                        string telefon = reader.GetString(reader.GetOrdinal("Phonenumber"));
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool AddClientToDatabase(Client record)
        {
            string insertQuery = @"INSERT INTO clients 
                                   (ClientName, PageURL, Phonenumber, Facebook, City, Data, Owner, Note)
                                   VALUES
                                   (@Klient, @StronaURL, @Telefon, @FacebookURL, @Miasto, @Data, @Wlasciciel, @Notatki)";
            using (MySqlCommand command = new MySqlCommand(insertQuery, conn))
            {
                command.Parameters.Clear();

                command.Parameters.AddWithValue("@Klient", record.ClientName);
                command.Parameters.AddWithValue("@StronaURL", record.PageURL);
                command.Parameters.AddWithValue("@Telefon", record.Phonenumber);
                command.Parameters.AddWithValue("@FacebookURL", record.Facebook);
                command.Parameters.AddWithValue("@Miasto", record.City);
                command.Parameters.AddWithValue("@Data", record.Data);
                command.Parameters.AddWithValue("@Wlasciciel", record.Owner);
                command.Parameters.AddWithValue("@Notatki", record.Note);
                int rowsAffected = 0;
                try
                {
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd przy próbie dodania {ex.Message}, {record.ClientName}, {record.Data}");
                }

                if (rowsAffected > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void CheckGridColumnAfterLoad(object sender, EventArgs e)
        {
            //foreach (var column in dataGridPreview.Columns)
            //{
            //    column.MaxWidth = 800;
            //    if (column is DataGridTextColumn textColumn && textColumn.ActualWidth > 150)
            //    {
            //        textColumn.Width = new DataGridLength(150);
            //    }
            //}
            //dataGridPreview.LayoutUpdated -= CheckGridColumnAfterLoad;
        }
    }
}
