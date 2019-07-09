using System;
using Domain.Settings;
using Microsoft.Extensions.Logging;

namespace Business.Engines
{
  public interface ILogEngine
  {
    void LogException(Exception exception);
  }

  public class LogEngine : ILogEngine
  {
    private readonly ILogger _logger;

    public LogEngine(GeneralSettings generalSettings, ILogger<LogEngine> logger)
    {
      _logger = logger;
    }

    public void LogException(Exception exception)
    {
      // Add your own log provider here
      _logger.LogError(exception.ToString(), "unexpected");
    }

  }
}