using Microsoft.Extensions.Logging;
using LagrangeLogLevel = Lagrange.Core.Event.EventArg.LogLevel;

namespace Columbidae.Server.Core;

public class Logging
{
    public ILogger Delegated { get; private set; }

    public Logging(string categoryName = "Program")
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        Delegated = loggerFactory.CreateLogger(categoryName);
    }

    public static Logging Default { get; } = new();
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