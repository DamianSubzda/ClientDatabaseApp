using System.Data.Entity;
using ClientDatabaseApp.Models;

public class PostgresContext : DbContext
{
    public PostgresContext() : base("name=PostgresContext")
    {
    }

    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Activity> Activities { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }


}