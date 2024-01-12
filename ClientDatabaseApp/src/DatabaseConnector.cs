using MySqlConnector;
using System;
using System.Windows;

namespace ClientDatabaseApp
{
    public class DatabaseConnector
    {
        private readonly string connectionString = "Server=aws.connect.psdb.cloud;Database=hvacclients;user=d1po4b3d8fa7tqbl0lvu;password=pscale_pw_4Ln3wb0AprJ67yTkAunynPtJ7YnaqBIA64xUyXRqaw5;SslMode=VerifyFull;";

        public MySqlConnection? ConnectToDatabase()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Close();
                return connection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
                return null;
            }
        }

    }
}

    
