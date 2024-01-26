using MySqlConnector;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClientDatabaseApp.View;
using ClientDatabaseApp.Model;
using System.Windows.Input;

namespace ClientDatabaseApp.ViewModel
{
    public class MVClientDatabaseMainWindow
    {
        private MySqlConnection conn = DatabaseConnector.connection;

        public ICommand ShowMoreDetailsCommand { get; private set; }
        public ICommand RemoveSelectedCommand { get; private set; }
        public ICommand AddFolowUpCommand { get; private set; }

        public MVClientDatabaseMainWindow()
        {
            ShowMoreDetailsCommand = new DelegateCommand<RoutedEventArgs>(ShowMoreDetails);
            RemoveSelectedCommand = new DelegateCommand<RoutedEventArgs>(RemoveSelected);
            AddFolowUpCommand = new DelegateCommand<RoutedEventArgs>(AddFolowUp);
            InitializeClientsGrid();
        }


        public void InitializeClientsGrid()
        {
            var context = new DBContextHVAC();
            var clients = context.ClientDBSet.ToList();

            MClientDatabase.DataGridClients = clients;
        }

        private void ShowMoreDetails(RoutedEventArgs e)
        {
            
        }

        private void RemoveSelected(RoutedEventArgs e)
        {

        }

        private void AddFolowUp(RoutedEventArgs e)
        {

        }

        void AddClient()
        {
            try
            {
                conn.Open();

                string insertQuery = "INSERT INTO `Client` " +
                    "(`ClientName`, `Phonenumber`, `Email`, `City`, `Facebook`, `Instagram`, `PageURL`, `Data`, `Owner`, `Note`) " +
                    "VALUES " +
                    "(@ClientName, @Phonenumber, @Email, @City, @Facebook, @Instagram, @PageURL, @Data, @Owner, @Note)";

                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
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
                conn.Close();
            }

            //var context = new DBContextHVAC();

            //Client c1 = new Client
            //{
            //    ClientName = "Pierwszy klient",
            //    Phonenumber = "100100100",
            //    Email = "@wp.pl",
            //    City = "Wawa",
            //    Facebook = ".pl",
            //    Instagram = ".pl",
            //    PageURL = ".pl",
            //    Data = DateTime.Now,
            //    Owner = "Ja",
            //    Note = "notatka"
            //};

            //context.ClientDBSet.Add(c1);
            //context.SaveChanges();


        }

        void AddFollowUp()
        {
            try
            {
                conn.Open();

                string insertQuery = "INSERT INTO `FollowUp` " +
                    "(`ClientId`, `Note`, `DateOfCreation`, `DateOfAction`) " +
                    "VALUES " +
                    "(@ClientId, @Note, @DateOfCreation, @DateOfAction)";

                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@ClientId", 1);
                cmd.Parameters.AddWithValue("@Note", "Notatka dla klienta");
                cmd.Parameters.AddWithValue("@DateOfCreation", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateOfAction", DateTime.Now.AddDays(3));

                cmd.ExecuteNonQuery();

                Console.WriteLine("Insert wykonany pomyślnie.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


    }
}

