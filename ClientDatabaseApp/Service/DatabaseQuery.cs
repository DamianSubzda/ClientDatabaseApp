using ClientDatabaseApp.Model;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientDatabaseApp.Service
{
    internal class DatabaseQuery
    {
        private MySqlConnection conn = DatabaseConnector.connection;

        List<(string, string)> exceptions;
        public DatabaseQuery()
        {
            exceptions = new List<(string, string)>();
        }

        //Zamienić później na full ORM z SaveChanges
        public List<(string, string)> TryAddClients(ObservableCollection<Client> clients)
        {
            int counter = 0;

            try
            {
                conn.Open();

                foreach (var item in clients)
                {
                    if (!CheckIfClientExists(item))
                    {
                        if (AddClient(item))
                        {
                            counter++;
                        }
                    }
                }
                exceptions.Add(("Executed", $"Dodano {counter}/{clients.Count} klientów"));

            }
            catch(MySqlException ex)
            {
                exceptions.Add(("Executed", $"Problem z połączniem z bazą danych:\n{ex.Message}"));
            }
            finally
            {
                conn.Close();
            }
            return exceptions;
        }
        public bool CheckIfClientExists(Client record)
        {
            string selectQuery = "SELECT * FROM Client" +
                                  " WHERE Facebook = @Facebook AND Phonenumber = @Phonenumber AND PageURL = @PageURL";

            using (MySqlCommand command = new MySqlCommand(selectQuery, conn))
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Facebook", record.Facebook);
                command.Parameters.AddWithValue("@Phonenumber", record.Phonenumber);
                command.Parameters.AddWithValue("@PageURL", record.PageURL);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        public bool AddClient(Client record)
        {
            string insertQuery = @"INSERT INTO Client 
                                   (ClientName, Phonenumber, Email, City, Facebook, Instagram, PageURL, Data, Owner, Note, Status)
                                   VALUES
                                   (@ClientName, @Phonenumber, @Email, @City, @Facebook, @Instagram, @PageURL, @Data, @Owner, @Note, @Status)";
            using (MySqlCommand command = new MySqlCommand(insertQuery, conn))
            {
                command.Parameters.Clear();

                command.Parameters.AddWithValue("@ClientName", record.ClientName);
                command.Parameters.AddWithValue("@Phonenumber", record.Phonenumber);
                command.Parameters.AddWithValue("@Email", record.Email);
                command.Parameters.AddWithValue("@City", record.City);
                command.Parameters.AddWithValue("@Facebook", record.Facebook);
                command.Parameters.AddWithValue("@Instagram", record.Instagram);
                command.Parameters.AddWithValue("@PageURL", record.PageURL);
                command.Parameters.AddWithValue("@Data", record.Data);
                command.Parameters.AddWithValue("@Owner", record.Owner);
                command.Parameters.AddWithValue("@Note", record.Note);
                command.Parameters.AddWithValue("@Status", record.Status);

                int rowsAffected = 0;
                try
                {
                    rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    exceptions.Add(("Exception", $"Błąd przy próbie dodania {record.ClientName}: \n {ex.Message}"));
                    return false;
                }
            }
        }
        public void AlterTable() //temp function
        {
            string dropForeignKeyQuery = @"ALTER TABLE FollowUp DROP FOREIGN KEY FollowUp_ibfk_1;";
            string addForeignKeyWithCascadeQuery = @"ALTER TABLE FollowUp ADD CONSTRAINT fk_clientId FOREIGN KEY (ClientId) REFERENCES Client(ClientId) ON DELETE CASCADE;";

            try
            {
                conn.Open();

                using (MySqlCommand command = new MySqlCommand(dropForeignKeyQuery, conn))
                {
                    command.ExecuteNonQuery();
                }
                using (MySqlCommand command = new MySqlCommand(addForeignKeyWithCascadeQuery, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"warning alter table: {ex.Message}");
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void ClearClientTable()
        {
            string deleteQuery = "DELETE FROM Client";
            try
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(deleteQuery, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(("Exception", $"Błąd przy próbie usunięcia rekordów tabeli Client: \n {ex.Message}"));
            }
            finally
            {
                conn.Close();
            }
        }
        public void ClearFollowUpTable()
        {
            string deleteQuery = "DELETE FROM FollowUp";
            try
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(deleteQuery, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(("Exception", $"Błąd przy próbie usunięcia rekordów tabeli Client: \n {ex.Message}"));
            }
            finally
            {
                conn.Close();
            }
        }
        public string DeleteClient(Client client)
        {
            string deleteQuery = @"DELETE FROM Client WHERE ClientId = @ClientId";
            try
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(deleteQuery, conn))
                {
                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@ClientId", client.ClientId);

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return $"Błąd przy próbie usunięcia klienta: \n {ex.Message}";
            }
            finally
            {
                conn.Close();
            }
            return "";
        }
        
        public void AddFollowUp(Client client, DateTime date, string Note)
        {
            try
            {
                conn.Open();

                string insertQuery = "INSERT INTO `FollowUp` " +
                    "(`ClientId`, `Note`, `DateOfCreation`, `DateOfAction`) " +
                    "VALUES " +
                    "(@ClientId, @Note, @DateOfCreation, @DateOfAction)";

                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@ClientId", client.ClientId);
                cmd.Parameters.AddWithValue("@Note", Note);
                cmd.Parameters.AddWithValue("@DateOfCreation", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateOfAction", date);

                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                //
            }
            finally
            {
                conn.Close();
            }
        }

        public string DeleteFollowUp(FollowUp SelectedFollowUp)
        {
            string deleteQuery = @"DELETE FROM FollowUp WHERE FollowUpId = @FollowUpId";
            try
            {
                conn.Open();
                using (MySqlCommand command = new MySqlCommand(deleteQuery, conn))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@FollowUpId", SelectedFollowUp.FollowUpId);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return $"Błąd przy próbie usunięcia followUp'a: \n {ex.Message}";
            }
            finally
            {
                conn.Close();
            }
            return "";
        }

    }
}
