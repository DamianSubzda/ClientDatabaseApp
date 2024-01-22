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
            string selectQuery = "SELECT * FROM clients";
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(selectQuery, connection))
            {
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGrid.ItemsSource = dataTable.DefaultView;
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

