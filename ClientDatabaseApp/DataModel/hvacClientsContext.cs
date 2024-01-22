using ClientDatabaseApp.DataModel.hvacclients;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.DataModel
{
    class hvacClientsContext : DbContext
    {
        public hvacClientsContext() : base("name=hvacContext")
        {

        }

        public DbSet<clients> Clients { get; set; }
        public DbSet<Follow_ups> Follow_Ups { get; set; }
    }
}
