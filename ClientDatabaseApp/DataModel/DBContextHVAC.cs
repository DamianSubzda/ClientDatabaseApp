using ClientDatabaseApp.DataModel.hvacclients;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel
{
    public class DBContextHVAC : DbContext
    {
        public DbSet<Client> ClientDBSet { get; set; }
        public DbSet<FollowUp> FollowUpDBSet { get; set; }

        public DBContextHVAC() : base("name=hvacConnectionString")
        {

        }
    }
}
