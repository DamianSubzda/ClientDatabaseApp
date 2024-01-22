using ClientDatabaseApp.DataModel;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClientDatabaseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DatabaseConnector db;
        private MySqlConnection connection;
        FollowUpData[] followups = new FollowUpData[31];

        public MainWindow()
        {
            db = new DatabaseConnector();
            connection = db.ConnectToDatabase();

            
            followups[0] = new FollowUpData(1, "jeden", "dwa");
            followups[1] = new FollowUpData(2, "dwa", "trzy");
            followups[2] = new FollowUpData(3, "trzy", "cztery", "piec");
            GetDaysFromMonth();

            using (var context = new hvacClientsContext())
            {
                var clients = context.Clients.ToList();
                var followUps = context.Follow_Ups.ToList();
            }


            InitializeComponent();
            InitializeCreatedElements();

            DataContext = this;
        }

        private void InitializeCreatedElements()
        {
            InitializeComboBoxStatus();
        }
        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (sender is TabControl tabControl && tabControl.SelectedItem is TabItem selectedTabItem)
            {
                if (selectedTabItem.Header.ToString() == "Baza klientów")
                {
                    InitializeClientsGrid();
                }
            }

        }
    }
}
