using Domain.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbMigration
{
  // This class is only for ef migration.
  public class MainContextFactory : IDesignTimeDbContextFactory<MainContext>
  {
    private static string _connectionString = "Server=localhost;Port=5433;Database=backend;User Id=postgres;Password=postgres";

    public MainContext CreateDbContext(string[] args)
    {
      var builder = new DbContextOptionsBuilder<MainContext>();
      builder.UseNpgsql(_connectionString);
      return new MainContext(builder.Options);
    }
  }
}