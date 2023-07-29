using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        modelBuilder.Entity<TenetUser>().HasKey(x=>x.Id);
        modelBuilder.Entity<TenetUser>().Property(c=>c.Id).HasConversion(id => JsonSerializer.Serialize(id,JsonSerializerOptions.Default)
            ,s => JsonSerializer.Deserialize<TenantId>(s,JsonSerializerOptions.Default)!);

        modelBuilder.Entity<TenetUser>().ToTable("Users", "tnt");


        base.OnModelCreating(modelBuilder);
    }
}