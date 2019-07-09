namespace Domain.Settings
{
  public class GeneralSettings
  {
    public string Environment { get; set; }
    public string MainDbConnectionString { get; set; }
    public string JwtKey { get; set; }
    public int JwtExpiresIn { get; set; } // Seconds
    public int RefreshTokenExpiresIn { get; set; } // Days
  }
}