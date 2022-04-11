using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB;

public class ApplicationContextEF : DbContext
{
    public DbSet<Session?> Session { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Computer> Computers { get; set; }
    public DbSet<FileEntity> File { get; set; }
    public DbSet<Employer> Employers { get; set; }

    public static MySqlServerVersion ServerVersion = new(new Version(8, 0, 26));

    public ApplicationContextEF(DbContextOptions<ApplicationContextEF> options)
        : base(options)
    {

    }
}