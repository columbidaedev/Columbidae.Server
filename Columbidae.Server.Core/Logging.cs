using Microsoft.Extensions.Logging;
using LagrangeLogLevel = Lagrange.Core.Event.EventArg.LogLevel;

namespace Columbidae.Server.Core;

public class Logging
{
    public readonly ILogger Delegated;

    private static LogLevel _logLevel = LogLevel.Debug;
    public static LogLevel LogLevel
    {
        get => _logLevel;
        set
        {
            _logLevel = value;
            Default = new Logging();
        }
    }

    public Logging(string categoryName = "Program")
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel));
        Delegated = loggerFactory.CreateLogger(categoryName);
    }
    
    public static Logging Default { get; private set;  } = new();
    public static ILogger Logger => Default.Delegated;
}

public static class LogConvert
{
    public static LogLevel ToMsLevel(this LagrangeLogLevel lvl)
    {
        return lvl switch
        {
            LagrangeLogLevel.Debug => LogLevel.Debug,
            LagrangeLogLevel.Exception => LogLevel.Critical,
            LagrangeLogLevel.Fatal => LogLevel.Critical,
            LagrangeLogLevel.Warning => LogLevel.Warning,
            LagrangeLogLevel.Information => LogLevel.Information,
            _ => LogLevel.None
        };
    }
}