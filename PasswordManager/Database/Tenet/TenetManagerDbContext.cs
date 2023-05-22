using Microsoft.EntityFrameworkCore;
using PasswordManager.Database.Tenet.Models;

namespace PasswordManager.Database.Tenet;

public class TenetManagerDbContext:DbContext
{
    public TenetManagerDbContext(DbContextOptions<TenetManagerDbContext> options) : base(options)
    {
    }

    public DbSet<TenetUser> TenetUsers => base.Set<TenetUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenetUser>().HasKey(c => c.Id);
        modelBuilder.Entity<TenetUser>().ToTable("Users", "tnt");


        base.OnModelCreating(modelBuilder);
    }
}