using NUnit.Framework;
using OpenQA.Selenium;
using NLog;
using Allure.Net.Commons;
using System.Runtime.InteropServices;
using AdvantageShopTestAutomation.Config;
using AdvantageShopTestAutomation.Utils;

namespace AdvantageShopTestAutomation.Tests.Base;

/// <summary>
/// Base test class with setup and teardown logic
/// Loads configuration from RunSettings and sets Allure environment variables
/// </summary>
[TestFixture]
public class BaseTest
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    protected IWebDriver Driver = null!;
    private string? _currentTestName;
    private string? _environment;
    private string? _browser;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Logger.Info("========== Test Suite Execution Started ==========");
        
        // Load RunSettings parameters
        var environmentParam = TestContext.Parameters.Get("environment", "Development");
        var browserParam = TestContext.Parameters.Get("browser", "chrome");
        var headlessParam = bool.TryParse(TestContext.Parameters.Get("headless", "false"), out var h) ? h : false;
        var timeoutParam = int.TryParse(TestContext.Parameters.Get("timeout", "15"), out var t) ? t : 15;

        // Override with environment variables if set (for CI/CD pipelines)
        _environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? environmentParam;
        _browser = Environment.GetEnvironmentVariable("BROWSER") ?? browserParam;
        
        // Initialize configuration with RunSettings parameters
        ConfigLoader.LoadRunSettingsParameters(_environment, _browser, headlessParam, timeoutParam);
        
        Logger.Info($"Environment: {ConfigLoader.GetEnvironment()}");
        Logger.Info($"Browser: {ConfigLoader.GetBrowser()}");
        Logger.Info($"Headless: {ConfigLoader.IsHeadless()}");
        Logger.Info($"Timeout: {ConfigLoader.GetExplicitWaitSeconds()}s");
        
        // Set Allure environment variables
        SetAllureEnvironment(_environment, _browser, headlessParam);
    }

    [SetUp]
    public void SetUp()
    {
        try
        {
            _currentTestName = TestContext.CurrentContext?.Test.Name ?? "UnknownTest";
            Logger.Info("========== Individual Test Started: {0} ==========", _currentTestName);
            Driver = BrowserConfig.InitializeBrowser();
            Logger.Info("WebDriver initialized successfully");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to initialize WebDriver");
            throw;
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (Driver != null)
            {
                // Take screenshot on failure - Check for failed test outcome
                var testResult = TestContext.CurrentContext?.Result;
                if (testResult?.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    CaptureScreenshot(_currentTestName ?? "UnknownTest");
                }

                BrowserConfig.QuitBrowser(Driver);
                Driver.Dispose();
                Logger.Info("========== Individual Test Completed ==========");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to clean up WebDriver");
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Logger.Info("========== Test Suite Execution Completed ==========");
    }

    /// <summary>
    /// Set Allure environment variables for reporting
    /// </summary>
    private void SetAllureEnvironment(string environment, string browser, bool headless)
    {
        try
        {
            AllureLifecycle.Instance.AddEnvironmentVariable("Environment", environment);
            AllureLifecycle.Instance.AddEnvironmentVariable("Browser", browser);
            AllureLifecycle.Instance.AddEnvironmentVariable("OS", GetOperatingSystem());
            AllureLifecycle.Instance.AddEnvironmentVariable("Framework", ".NET 10");
            AllureLifecycle.Instance.AddEnvironmentVariable("Execution Mode", headless ? "Headless" : "Headed");
            AllureLifecycle.Instance.AddEnvironmentVariable("Execution Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss UTC"));
            AllureLifecycle.Instance.AddEnvironmentVariable("Application URL", GetApplicationUrl(environment));
            
            Logger.Info("Allure environment variables set successfully");
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Failed to set Allure environment variables - Allure may not be initialized");
        }
    }

    /// <summary>
    /// Get operating system information
    /// </summary>
    private string GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return "Windows";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return "Linux";
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return "macOS";
        return "Unknown";
    }

    /// <summary>
    /// Get application URL based on environment
    /// </summary>
    private string GetApplicationUrl(string environment)
    {
        return environment.ToLower() switch
        {
            "production" => "https://www.advantageonlineshopping.com/",
            "staging" => "https://staging.advantageonlineshopping.com/",
            "development" => "http://localhost:3000/",
            _ => "https://www.advantageonlineshopping.com/"
        };
    }

    /// <summary>
    /// Capture screenshot on test failure
    /// </summary>
    protected void CaptureScreenshot(string testName)
    {
        if (!string.IsNullOrEmpty(testName) && Driver != null)
        {
            ScreenshotUtils.TakeScreenshot(Driver, testName);
        }
    }

    /// <summary>
    /// Wait for specified seconds
    /// </summary>
    protected void WaitSeconds(int seconds)
    {
        Logger.Debug($"Waiting for {seconds} seconds");
        System.Threading.Thread.Sleep(seconds * 1000);
    }
}