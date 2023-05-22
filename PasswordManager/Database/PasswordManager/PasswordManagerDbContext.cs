using Microsoft.EntityFrameworkCore;
using PasswordManager.Database.PasswordManager.Models;
using PasswordManager.Database.Tenet.Models;
using PasswordManager.Database.Tenet;

namespace PasswordManager.Database.PasswordManager;

public class PasswordManagerDbContext:DbContext
{
    public PasswordManagerDbContext(DbContextOptions<PasswordManagerDbContext> options) : base(options)
    {
    }

    public DbSet<UserPassword> UserPasswords => base.Set<UserPassword>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPassword>().HasKey(c => c.Id);
        modelBuilder.Entity<UserPassword>().ToTable("passwords", "usr");


        base.OnModelCreating(modelBuilder);
    }
}