using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB;

public class ApplicationContextEF : DbContext
{
    public ApplicationContextEF()
    {

    }

    public DbSet<Session?> Session { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Computer> Computers { get; set; }
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<Employer> Employers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(MySQLDatabaseContext.ConnectionString, new MySqlServerVersion(new Version(8, 0, 26)));
    }
}