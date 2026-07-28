using NUnit.Framework;
using OpenQA.Selenium;
using NLog;
using AdvantageShopTestAutomation.Config;
using AdvantageShopTestAutomation.Utils;

namespace AdvantageShopTestAutomation.Tests.Base;

/// <summary>
/// Base test class with setup and teardown logic
/// Loads configuration from RunSettings
/// </summary>
[TestFixture]
public class BaseTest
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    protected IWebDriver Driver = null!;
    private string? _currentTestName;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Logger.Info("========== Test Suite Execution Started ==========");
        
        // Load RunSettings parameters
        var environmentParam = TestContext.Parameters.Get("environment", "Development");
        var browserParam = TestContext.Parameters.Get("browser", "chrome");
        var headlessParam = bool.TryParse(TestContext.Parameters.Get("headless", "false"), out var h) ? h : false;
        var timeoutParam = int.TryParse(TestContext.Parameters.Get("timeout", "15"), out var t) ? t : 15;

        // Initialize configuration with RunSettings parameters
        ConfigLoader.LoadRunSettingsParameters(environmentParam, browserParam, headlessParam, timeoutParam);
        
        Logger.Info($"Environment: {ConfigLoader.GetEnvironment()}");
        Logger.Info($"Browser: {ConfigLoader.GetBrowser()}");
        Logger.Info($"Headless: {ConfigLoader.IsHeadless()}");
        Logger.Info($"Timeout: {ConfigLoader.GetExplicitWaitSeconds()}s");
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