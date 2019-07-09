namespace Business.Models
{
  public class AccessGrantDto
  {
    public string TokenType { get; set; }
    public string AccessToken { get; set; }
    public int JwtExpiresIn { get; set; }
    public string RefreshToken { get; set; }
  }
}