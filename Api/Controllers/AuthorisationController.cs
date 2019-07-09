using Business.Managers;
using Domain.DbContexts;
using Domain.Settings;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  public class AuthorisationController : BaseController
  {
    private readonly IAuthorisationManager _authorisationManager;
    private readonly MainContext _mainContext;
    private readonly GeneralSettings _generalSettings;

    public AuthorisationController(IAuthorisationManager authorisationManager, MainContext mainContext, GeneralSettings generalSettings)
    {
      _authorisationManager = authorisationManager;
      _mainContext = mainContext;
      _generalSettings = generalSettings;
    }

    [AllowAnonymous]
    [HttpPost("Refresh")]
    public ActionResult<AccessGrantDto> RefreshToken(RefreshTokenDto refreshTokenRequest)
    {
      return _authorisationManager.RefreshToken(refreshTokenRequest);
    }

    [AllowAnonymous]
    [HttpPost("Token")]
    public ActionResult<AccessGrantDto> CreateToken(LoginDto loginDto)
    {
      var user = _authorisationManager.Authenticate(loginDto);

      if (user == null)
      {
        return Unauthorized();
      }

      return _authorisationManager.CreateAccessGrant(user);
    }

    [AllowAnonymous]
    [HttpGet("Ping")]
    public ActionResult<string> Ping()
    {
      return "Hello";
    }
  }
}