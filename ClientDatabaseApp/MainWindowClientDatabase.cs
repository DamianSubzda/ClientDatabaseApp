using CsvHelper;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;

namespace ClientDatabaseApp
{
    public partial class MainWindow : Window
    {

        private void InitializeClientsGrid()
        {
            string selectQuery = "SELECT ClientId as `Id`, ClientName as `Klient`, Phonenumber as `Numer telefonu` FROM Client";

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        void AddClient()
        {
            try
            {
                connection.Open();

                string insertQuery = "INSERT INTO `Client` " +
                    "(`ClientName`, `Phonenumber`, `Email`, `City`, `Facebook`, `Instagram`, `PageURL`, `Data`, `Owner`, `Note`) " +
                    "VALUES " +
                    "(@ClientName, @Phonenumber, @Email, @City, @Facebook, @Instagram, @PageURL, @Data, @Owner, @Note)";

                MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                cmd.Parameters.AddWithValue("@ClientName", "Nazwa klienta");
                cmd.Parameters.AddWithValue("@Phonenumber", "123456789");
                cmd.Parameters.AddWithValue("@Email", "klient@example.com");
                cmd.Parameters.AddWithValue("@City", "Miasto");
                cmd.Parameters.AddWithValue("@Facebook", "https://www.facebook.com/klient");
                cmd.Parameters.AddWithValue("@Instagram", "https://www.instagram.com/klient");
                cmd.Parameters.AddWithValue("@PageURL", "https://www.example.com/klient");
                cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                cmd.Parameters.AddWithValue("@Owner", "Właściciel");
                cmd.Parameters.AddWithValue("@Note", "Notatka dla klienta");

                cmd.ExecuteNonQuery();

                Console.WriteLine("Insert wykonany pomyślnie.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void ShowMoreDetails(object sender, RoutedEventArgs e)
        {

        }

        private void EditSelected(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveSelected(object sender, RoutedEventArgs e)
        {

        }

        private void AddFolowUp(object sender, RoutedEventArgs e)
        {

        }

    }
}

