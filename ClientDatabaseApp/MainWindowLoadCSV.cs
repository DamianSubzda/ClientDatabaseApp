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
using ClientDatabaseApp.DataModel.hvacclients;

namespace ClientDatabaseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private void GetDataFromCSV(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFileDialog = new OpenFileDialog
        //    {
        //        Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
        //        Title = "Wybierz plik CSV"
        //    };

        //    if (openFileDialog.ShowDialog() == true)
        //    {
        //        string filePath = openFileDialog.FileName;

        //        using (var reader = new StreamReader(filePath))
        //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //        {

        //            var records = csv.GetRecords<ClientData>().ToList();
        //            dataGridPreview.ItemsSource = records;
        //        }
        //    }
        //    dataGridPreview.LayoutUpdated += CheckGridColumnAfterLoad;
        //}

        //private void PassDataToDatabase(object sender, RoutedEventArgs e)
        //{
        //    int counter = 0;
        //    if (connection != null)
        //    {
        //        connection.Open();
        //        if (dataGridPreview.ItemsSource is IEnumerable<ClientData> items)
        //        {
        //            foreach (var item in items)
        //            {
        //                if (!CheckIfClientIsInDatabase(item))
        //                {
        //                    if (AddClientToDatabase(item))
        //                    {
        //                        counter++;
        //                    }
        //                }
        //            }
        //        }
        //        connection.Close();
        //        MessageBox.Show($"Dodano {counter}/{dataGridPreview.Items.Count}", "Notka");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Brak połączenia z bazą danych");
        //        return;
        //    }
        //}

        //private bool CheckIfClientIsInDatabase(Client record)
        //{
        //    string selectQuery = "SELECT * FROM clients" +
        //                          " WHERE FacebookURL = @FacebookURL AND Telefon = @Telefon";
        //    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
        //    {
        //        command.Parameters.Clear();
        //        command.Parameters.AddWithValue("@FacebookURL", record.FacebookURL);
        //        command.Parameters.AddWithValue("@Telefon", record.Telefon);
        //        using (MySqlDataReader reader = command.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                int idclients = reader.GetInt32(reader.GetOrdinal("idclients"));
        //                string klientName = reader.GetString(reader.GetOrdinal("Klient"));
        //                string facebookURL = reader.GetString(reader.GetOrdinal("FacebookURL"));
        //                string telefon = reader.GetString(reader.GetOrdinal("Telefon"));
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        //private bool AddClientToDatabase(ClientData record)
        //{
        //    string insertQuery = @"INSERT INTO clients 
        //                           (Klient, StronaURL, Telefon, FacebookURL, Miasto, Data, Wlasciciel, FollowUp1, FollowUp2, Notatki)
        //                           VALUES
        //                           (@Klient, @StronaURL, @Telefon, @FacebookURL, @Miasto, @Data, @Wlasciciel, @FollowUp1, @FollowUp2, @Notatki)";
        //    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
        //    {
        //        command.Parameters.Clear();

        //        command.Parameters.AddWithValue("@Klient", record.Klient);
        //        command.Parameters.AddWithValue("@StronaURL", record.StronaURL);
        //        command.Parameters.AddWithValue("@Telefon", record.Telefon);
        //        command.Parameters.AddWithValue("@FacebookURL", record.FacebookURL);
        //        command.Parameters.AddWithValue("@Miasto", record.Miasto);
        //        if (record.Data != "")
        //        {
        //            command.Parameters.AddWithValue("@Data", DateTime.Parse(record.Data));
        //        }
        //        else
        //        {
        //            command.Parameters.AddWithValue("@Data", null);
        //        }
                
        //        command.Parameters.AddWithValue("@Wlasciciel", record.Wlasciciel);
        //        command.Parameters.AddWithValue("@FollowUp1", record.FollowUp1);
        //        command.Parameters.AddWithValue("@FollowUp2", record.FollowUp2);
        //        command.Parameters.AddWithValue("@Notatki", record.Notatki);
        //        int rowsAffected = 0;
        //        try
        //        {
        //            rowsAffected = command.ExecuteNonQuery();
        //        }
        //        catch(Exception ex)
        //        {
        //            MessageBox.Show($"Błąd przy próbie dodania {ex.Message}, {record.Klient}, {record.Data}");
        //        }
                
        //        if (rowsAffected > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        private void CheckGridColumnAfterLoad(object sender, EventArgs e)
        {
            foreach (var column in dataGridPreview.Columns)
            {
                column.MaxWidth = 800;
                if (column is DataGridTextColumn textColumn && textColumn.ActualWidth > 150)
                {
                    textColumn.Width = new DataGridLength(150);
                }
            }
            dataGridPreview.LayoutUpdated -= CheckGridColumnAfterLoad;
        }
    }
}
