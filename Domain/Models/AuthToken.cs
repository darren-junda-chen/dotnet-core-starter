using System;

namespace Domain.Models
{
  public class AuthToken
  {
    public int Id { get; set; }
    public string RereshToken { get; set; }
    public DateTime Expiry { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
  }
}