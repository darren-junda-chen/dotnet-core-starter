using System.Linq;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts
{
  public class MainContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<AuthToken> AuthTokens { get; set; }

    public MainContext(DbContextOptions<MainContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      // Disable cascade deletion
      foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
      {
        relationship.DeleteBehavior = DeleteBehavior.Restrict;
      }
      
      // Configs
      GeneralConfiguration.Config(modelBuilder);
      MembershipConfiguration.Config(modelBuilder);

      // Data seed
      SeedData.Seed(modelBuilder);
    }
  }
}