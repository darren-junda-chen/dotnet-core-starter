using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Business.Engines
{
  public interface IPasswordHashEngine
  {
    string GenerateSalt();
    string GeneratePasswordHash(string password, string salt);
    bool ComparePassword(string salt, string providedHash, string password);
  }

  public class PasswordHashEngine : IPasswordHashEngine
  {
    public string GenerateSalt()
    {
      byte[] salt = new byte[128 / 8];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(salt);
      }
      return Convert.ToBase64String(salt);
    }

    public string GeneratePasswordHash(string password, string salt)
    {
      string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                          password: password,
                          salt: Convert.FromBase64String(salt),
                          prf: KeyDerivationPrf.HMACSHA1,
                          iterationCount: 10000,
                          numBytesRequested: 256 / 8)
                      );
      return hashed;
    }

    public bool ComparePassword(string salt, string providedHash, string password)
    {
      var hash = GeneratePasswordHash(password, salt);
      return hash == providedHash;
    }
  }
}