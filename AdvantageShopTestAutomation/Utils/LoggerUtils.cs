using NLog;

namespace AdvantageShopTestAutomation.Utils;

public static class LoggerUtils
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public static void LogInfo(string message) => Logger.Info(message);

    public static void LogWarning(string message) => Logger.Warn(message);

    public static void LogError(string message, Exception ex) => Logger.Error(ex, message);

    public static void LogDebug(string message) => Logger.Debug(message);

    public static void LogTrace(string message) => Logger.Trace(message);
}