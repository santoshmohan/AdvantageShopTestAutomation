using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using NLog;

namespace AdvantageShopTestAutomation.Config;

public class BrowserConfig
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public static IWebDriver InitializeBrowser()
    {
        try
        {
            string browserName = ConfigLoader.GetBrowser();
            IWebDriver driver = browserName.ToLower().Trim() switch
            {
                "chrome" => InitializeChrome(),
                "firefox" => InitializeFirefox(),
                _ => throw new ArgumentException($"Unsupported browser: {browserName}")
            };

            driver.Manage().Window.Maximize();
            Logger.Info($"Browser {browserName} initialized for environment: {ConfigLoader.GetEnvironment()}");

            return driver;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to initialize browser");
            throw;
        }
    }

    private static IWebDriver InitializeChrome()
    {
        try
        {
            new DriverManager().SetUpDriver(new ChromeConfig());

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            if (ConfigLoader.IsHeadless())
            {
                chromeOptions.AddArgument("--headless");
            }

            var driver = new ChromeDriver(chromeOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            Logger.Info("Chrome browser initialized successfully");
            return driver;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to initialize Chrome: {ex.Message}");
            throw;
        }
    }

private static IWebDriver InitializeFirefox()
{
    try
    {
        var firefoxOptions = new FirefoxOptions();
        
        // Enable headless if configured
        if (ConfigLoader.IsHeadless())
        {
            firefoxOptions.AddArgument("--headless");
        }
        
        // Additional useful options
        firefoxOptions.AddArgument("--no-sandbox");
        firefoxOptions.SetPreference("browser.download.folderList", 2);
        
        var driver = new FirefoxDriver(firefoxOptions);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(40);
        
        Logger.Info("Firefox browser initialized successfully");
        return driver;
    }
    catch (Exception ex)
    {
        Logger.Error(ex,$"Failed to initialize Firefox: {ex.Message}");
        throw;
    }
}

    public static void QuitBrowser(IWebDriver driver)
    {
        try
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                Logger.Info("Browser closed successfully");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to close browser");
        }
    }
}