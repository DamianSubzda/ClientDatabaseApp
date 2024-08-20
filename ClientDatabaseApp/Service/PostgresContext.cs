using System.Data.Entity;
using ClientDatabaseApp.Model;

public class PostgresContext : DbContext
{
    public PostgresContext() : base("name=PostgresContext")
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Activity> Activities { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}