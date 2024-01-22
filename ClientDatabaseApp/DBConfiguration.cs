﻿using System.Data.Entity;
using MySql.Data.EntityFramework;
using MySql.Data.MySqlClient;

namespace ClientDatabaseApp
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
