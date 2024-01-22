using ClientDatabaseApp.DataModel.hvacclients;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel
{
    public class hvacClientsContext : DbContext
    {
        public hvacClientsContext() : base("name=hvacContext")
        {

        }

        public DbSet<Client> ClientsDBSet { get; set; }
        public DbSet<FollowUp> FollowUpsDBSet { get; set; }
    }
}
