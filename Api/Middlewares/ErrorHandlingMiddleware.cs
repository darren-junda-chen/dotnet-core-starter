using System;
using System.Net;
using System.Threading.Tasks;
using Business.Engines;
using Business.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Api.Middlewares
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
      this.next = next;
    }

    public async Task Invoke(HttpContext context, ILogEngine logEngine)
    {
      try
      {
        await next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex, logEngine);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogEngine logEngine)
    {
      var code = HttpStatusCode.InternalServerError;
      var error = exception.Message;

      if (exception is ResourceExistedException) { code = HttpStatusCode.Conflict; }
      else if (exception is PaymentRequiredException) { code = HttpStatusCode.PaymentRequired; }
      else if (exception is UnauthorizedException) { code = HttpStatusCode.Unauthorized; }
      else if (exception is NotAllowedException) { code = HttpStatusCode.Forbidden; }
      else if (exception is NotFoundException) { code = HttpStatusCode.NotFound; }
      else if (exception is InvalidParameterException) { code = HttpStatusCode.BadRequest; }
      else if (exception is DbUpdateConcurrencyException) { code = HttpStatusCode.RequestTimeout; error = "Db update concurrent conflict. Please retry"; }
      else
      {
        logEngine.LogException(exception);
        error = "Unexpected error";
      }

      var result = JsonConvert.SerializeObject(new { error = error });
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)code;
      await context.Response.WriteAsync(result);
    }
  }
}