using MySqlConnector;
using System;
using System.Collections.Generic;
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


        public List<string> Days { get; set; }

        public MainWindow()
        {
            db = new DatabaseConnector();
            connection = db.ConnectToDatabase();

            GetDaysFromMonth(2, 2024);

            InitializeComponent();
            InitializeCreatedElements();

            DataContext = this;
        }

        private void InitializeCreatedElements()
        {
            InitializeComboBoxStatus();
        }
        enum DaysEnum
        {
            Poniedziałek, Wtorek, Środa, Czwartek, Piątek, Sobota, Niedziela
        }
        private void GetDaysFromMonth(int monthNumber, int yearNumber)
        {
            int days = DateTime.DaysInMonth(yearNumber, monthNumber);

            DateTime firstDayOfMonth = new DateTime(yearNumber, monthNumber, 1);
            DayOfWeek dayOfWeek = firstDayOfMonth.DayOfWeek;
            DaysEnum enumStart = (DaysEnum)dayOfWeek - 1;

            Days = new List<string>();

            for (int i = 0; i < (int)enumStart ; i++)
            {
                Days.Add("");
            }

            for (int i = 1; i <= days; i++)
            {
                Days.Add(i.ToString() + ". " + enumStart.ToString());
                enumStart = (DaysEnum)(((int)enumStart + 1) % 7);
            }
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
