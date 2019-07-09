using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts
{
  public static class MembershipConfiguration
  {
    public static void Config(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasIndex(x => x.Email)
        .IsUnique();
    }
  }
}