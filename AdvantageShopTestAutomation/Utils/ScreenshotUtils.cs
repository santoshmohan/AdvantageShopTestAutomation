using OpenQA.Selenium;
using NLog;

namespace AdvantageShopTestAutomation.Utils;

public class ScreenshotUtils
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private static readonly string ScreenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

    static ScreenshotUtils()
    {
        if (!Directory.Exists(ScreenshotDir))
        {
            Directory.CreateDirectory(ScreenshotDir);
            Logger.Info($"Screenshot directory created: {ScreenshotDir}");
        }
    }

    public static string TakeScreenshot(IWebDriver driver, string testName)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            var fileName = $"{testName}_{timestamp}.png";
            var filePath = Path.Combine(ScreenshotDir, fileName);
            
            screenshot.SaveAsFile(filePath);
            Logger.Info($"Screenshot saved: {filePath}");
            
            return filePath;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to take screenshot");
            return string.Empty;
        }
    }
}