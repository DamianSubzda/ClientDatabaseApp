using System;
using System.Collections.Generic;
using System.Data.Entity;
using ClientDatabaseApp.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDatabaseApp.Service
{
    //[DbConfigurationType(typeof(DbConfiguration))]
    public class DBContextHVAC : DbContext
    {

        public DBContextHVAC() : base("name=MyConnectionString")
        {
            
        }

        public DbSet<Client> ClientDBSet { get; set; }
        public DbSet<FollowUp> FollowUpDBSet { get; set; }
    }
}
