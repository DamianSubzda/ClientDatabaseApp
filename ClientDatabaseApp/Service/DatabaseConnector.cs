﻿using MySqlConnector;
using System.Configuration;

namespace ClientDatabaseApp.Service
{
    public sealed class DatabaseConnector
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        private static DatabaseConnector _instance;
        public static MySqlConnection connection;

        public DatabaseConnector()
        {
            connection = new MySqlConnection(connectionString);
        }
        public static DatabaseConnector GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseConnector();
                return _instance;
            }
            return _instance;
        }
    }
}


