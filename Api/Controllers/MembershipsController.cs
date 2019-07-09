using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Business.Exceptions;
using Business.Managers;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  public class MembershipsController : BaseController
  {
    private readonly IMembershipsManager _membershipsManager;
    private readonly IAuthorisationManager _authorisationManager;

    public MembershipsController(IMembershipsManager membershipsManager, IAuthorisationManager authorisationManager)
    {
      _membershipsManager = membershipsManager;
      _authorisationManager = authorisationManager;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public AccessGrantDto Register(RegisterDto register)
    {
      var user = _membershipsManager.Register(register, false);
      return _authorisationManager.CreateAccessGrant(user);
    }

    [HttpPut("ChangePassword")]
    public ActionResult<AccessGrantDto> ChangePassword(ChangePasswordDto changePasswordRequest)
    {
      var user = _membershipsManager.ChangePassword(UserId, changePasswordRequest);
      return _authorisationManager.CreateAccessGrant(user);
    }
  }
}