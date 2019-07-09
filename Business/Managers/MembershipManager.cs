using System.Linq;
using AutoMapper;
using Business.Engines;
using Business.Exceptions;
using Business.Models;
using Domain.DbContexts;
using Domain.Models;
using Domain.Settings;

namespace Business.Managers
{
  public interface IMembershipsManager
  {
    User Register(RegisterDto register, bool commitChanges);
    User ChangePassword(int userId, ChangePasswordDto changePasswordDto);
  }

  public class MembershipsManager : IMembershipsManager
  {
    private readonly MainContext _mainContext;
    private readonly GeneralSettings _generalSettings;
    private readonly IPasswordHashEngine _passwordHashEngine;
    private readonly IMapper _mapper;

    public MembershipsManager(MainContext mainContext, GeneralSettings generalSettings, IPasswordHashEngine passwordHashEngine, IMapper mapper)
    {
      _mainContext = mainContext;
      _generalSettings = generalSettings;
      _passwordHashEngine = passwordHashEngine;
      _mapper = mapper;
    }

    public User Register(RegisterDto register, bool commitChanges)
    {
      if (_mainContext.Users.Any(u => u.Email == register.Email))
      {
        throw new ResourceExistedException("Email existed");
      }

      var passwordSalt = _passwordHashEngine.GenerateSalt();
      var user = new User
      {
        Email = register.Email,
        UserName = register.UserName,
        Password = _passwordHashEngine.GeneratePasswordHash(register.Password, passwordSalt),
        PasswordSalt = passwordSalt,
      };
      _mainContext.Users.Add(user);

      if (commitChanges)
      {
        _mainContext.SaveChanges();
      }

      return user;
    }

    public User ChangePassword(int userId, ChangePasswordDto changePasswordDto)
    {
      var user = _mainContext.Users.Find(userId);
      if (user == null)
      {
        throw new NotFoundException("No user found");
      }

      if (!_passwordHashEngine.ComparePassword(user.PasswordSalt, user.Password, changePasswordDto.oldPassword))
      {
        throw new UnauthorizedException("Wrong password");
      }

      // Reset password
      var passwordSalt = _passwordHashEngine.GenerateSalt();
      user.PasswordSalt = passwordSalt;
      user.Password = _passwordHashEngine.GeneratePasswordHash(changePasswordDto.newPassword, passwordSalt);

      ClearRefreshToken(user.Id);
      return user;
    }

    private void ClearRefreshToken(int userId)
    {
      _mainContext.AuthTokens.RemoveRange(_mainContext.AuthTokens.Where(a => a.User.Id == userId));
      _mainContext.SaveChanges();
    }
  }
}