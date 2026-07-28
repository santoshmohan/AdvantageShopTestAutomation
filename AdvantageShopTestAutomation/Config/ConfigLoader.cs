using Microsoft.Extensions.Configuration;
using NLog;

namespace AdvantageShopTestAutomation.Config;

public class ConfigLoader
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private static IConfiguration? _configuration;
    private static EnvironmentType _currentEnvironment = EnvironmentType.Development;
    private static string _browser = "firefox";
    private static bool _headless = false;
    private static int _timeout = 15;


    public static void Initialize(EnvironmentType environment)
    {
        try
        {
            _currentEnvironment = environment;
            var configFileName = $"appsettings.{environment}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(configFileName, optional: false, reloadOnChange: true);

            _configuration = builder.Build();
            
            Logger.Info($"Configuration loaded for environment: {environment}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to load configuration for environment: {environment}");
            throw new InvalidOperationException($"Failed to load configuration for {environment}", ex);
        }
    }

    public static void LoadRunSettingsParameters(
        string? environment = null,
        string? browser = null,
        bool? headless = null,
        int? timeout = null)
    {
        try
        {
            // Set environment
            if (!string.IsNullOrEmpty(environment) && Enum.TryParse<EnvironmentType>(environment, true, out var env))
            {
                _currentEnvironment = env;
                Initialize(env);
            }

            // Set browser
            if (!string.IsNullOrEmpty(browser))
            {
                _browser = browser.ToLower();
            }

            // Set headless mode
            if (headless.HasValue)
            {
                _headless = headless.Value;
            }

            // Set timeout
            if (timeout.HasValue && timeout.Value > 0)
            {
                _timeout = timeout.Value;
            }

            Logger.Info($"RunSettings loaded - Environment: {_currentEnvironment}, Browser: {_browser}, Headless: {_headless}, Timeout: {_timeout}s");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to load RunSettings parameters");
        }
    }


    public static string GetSetting(string key)
    {
        if (_configuration == null)
        {
            Initialize(EnvironmentType.Development);
        }

        var value = _configuration?[key];
        if (string.IsNullOrEmpty(value))
        {
            Logger.Warn($"Configuration key not found: {key}");
        }

        return value ?? string.Empty;
    }


    public static string GetBaseUrl() => GetSetting("app:baseUrl");

    /// <summary>
    /// Get explicit wait timeout in seconds
    /// </summary>
    public static int GetExplicitWaitSeconds() => _timeout;


    public static string GetBrowser() => _browser;


    public static bool IsHeadless() => _headless;


    public static EnvironmentType GetEnvironment() => _currentEnvironment;

 
    public static string GetLogLevel() => GetSetting("app:logLevel");
}