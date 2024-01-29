using System.Data.Entity;
using ClientDatabaseApp.Model;

namespace ClientDatabaseApp.Service
{
    [DbConfigurationType(typeof(DbConfiguration))]
    public class DBContextHVAC : DbContext
    {
        public DbSet<Client> ClientDBSet { get; set; }
        public DbSet<FollowUp> FollowUpDBSet { get; set; }

        public DBContextHVAC() : base("name=hvacConnectionString")
        {

        }
    }
}
