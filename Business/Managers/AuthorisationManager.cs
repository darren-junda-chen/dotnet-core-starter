using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Business.Engines;
using Business.Exceptions;
using Business.Models;
using Common.Constants;
using Domain.DbContexts;
using Domain.Models;
using Domain.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Business.Managers
{
  public interface IAuthorisationManager
  {
    User Authenticate(LoginDto login);
    AccessGrantDto CreateAccessGrant(User user);
    AccessGrantDto RefreshToken(RefreshTokenDto refreshTokenRequest);
  }

  public class AuthorisationManager : IAuthorisationManager
  {
    private readonly GeneralSettings _generalSettings;
    private readonly MainContext _mainContext;
    private readonly IPasswordHashEngine _passwordHashEngine;

    public AuthorisationManager(GeneralSettings generalSettings, MainContext mainContext, IPasswordHashEngine passwordHashEngine)
    {
      _generalSettings = generalSettings;
      _mainContext = mainContext;
      _passwordHashEngine = passwordHashEngine;
    }

    public User Authenticate(LoginDto loginDto)
    {
      var user = _mainContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
      if (user == null)
      {
        return null;
      }
      if (!_passwordHashEngine.ComparePassword(user.PasswordSalt, user.Password, loginDto.Password))
      {
        return null;
      }
      return user;
    }

    public AccessGrantDto RefreshToken(RefreshTokenDto refreshTokenDto)
    {
      // Validate old access token.
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_generalSettings.JwtKey))
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken securityToken = null;
      IEnumerable<Claim> claims = null;
      try
      {
        claims = tokenHandler.ValidateToken(refreshTokenDto.AccessToken, tokenValidationParameters, out securityToken).Claims;
      }
      catch (Exception)
      {
        throw new UnauthorizedException("Invalid access token");
      }

      var jwtSecurityToken = securityToken as JwtSecurityToken;
      if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new UnauthorizedException("Invalid access token");
      }

      // Validate refresh token
      var authTokenId = claims.FirstOrDefault(c => c.Type == AuthClaimTypes.AuthTokenId).Value;
      var authToken = _mainContext.AuthTokens.Find(int.Parse(authTokenId));
      if (authToken == null || authToken.RereshToken != refreshTokenDto.RefreshToken || authToken.Expiry < DateTime.Now)
      {
        throw new UnauthorizedException("Invalid refresh token.");
      }

      // Update auth token in database
      var refreshToken = GenerateRefreshToken();
      authToken.RereshToken = refreshToken;
      authToken.Expiry = DateTime.Now.AddDays(_generalSettings.RefreshTokenExpiresIn);
      _mainContext.SaveChanges();

      // Generate access token
      var accessToken = GenerateAccessToken(claims.ToArray());
      var userId = int.Parse(claims.FirstOrDefault(c => c.Type == AuthClaimTypes.UserId).Value);

      return new AccessGrantDto
      {
        TokenType = "Bearer",
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        JwtExpiresIn = _generalSettings.JwtExpiresIn
      };
    }

    public AccessGrantDto CreateAccessGrant(User user)
    {
      // Generate and store refresh token
      var refreshToken = GenerateRefreshToken();

      var authToken = new AuthToken
      {
        RereshToken = refreshToken,
        User = user,
        Expiry = DateTime.Now.AddDays(_generalSettings.RefreshTokenExpiresIn)
      };
      _mainContext.AuthTokens.Add(authToken);

      _mainContext.SaveChanges();

      // Generate access token
      var claims = new[]{
                new Claim(AuthClaimTypes.AuthTokenId, authToken.Id.ToString()),
                new Claim(AuthClaimTypes.UserId, user.Id.ToString())
            };
      var accessToken = GenerateAccessToken(claims);

      return new AccessGrantDto
      {
        TokenType = "Bearer",
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        JwtExpiresIn = _generalSettings.JwtExpiresIn
      };
    }

    private string GenerateAccessToken(Claim[] claims)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_generalSettings.JwtKey));
      var jwt = new JwtSecurityToken(
                          claims: claims,
                          expires: DateTime.Now.AddSeconds(_generalSettings.JwtExpiresIn),
                          signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                      );
      var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
      return accessToken;
    }

    private string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }
  }
}