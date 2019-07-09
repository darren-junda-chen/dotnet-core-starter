using System.Linq;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public abstract class BaseController : ControllerBase
  {
    protected int UserId
    {
      get
      {
        if (!HttpContext.User.HasClaim(c => c.Type == AuthClaimTypes.UserId))
        {
          return 0;
        }

        return int.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == AuthClaimTypes.UserId).Value);
      }
      private set { }
    }
  }
}