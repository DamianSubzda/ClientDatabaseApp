using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;

namespace ClientDatabaseApp.ViewModel
{
    public class DbConfiguration : System.Data.Entity.DbConfiguration
    {
        public DbConfiguration()
        {
            SetProviderServices("MySql.Data.MySqlClient", new MySqlProviderServices());
            SetDefaultConnectionFactory(new MySqlConnectionFactory());
            SetMigrationSqlGenerator(
            "MySql.Data.MySqlClient",
            () => new MySqlMigrationSqlGenerator());
        }
    }
}
