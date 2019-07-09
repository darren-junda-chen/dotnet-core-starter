using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts
{
  public static class GeneralConfiguration
  {
    public static void Config(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AuthToken>()
        .HasOne(x => x.User)
        .WithMany()
        .HasForeignKey(x => x.UserId)
        .IsRequired();
    }
  }
}