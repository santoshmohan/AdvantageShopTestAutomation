using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NLog;
using AdvantageShopTestAutomation.Config;

namespace AdvantageShopTestAutomation.Pages;

/// <summary>
/// Base page class with common methods for all page objects
/// </summary>
public class BasePage
{
    protected readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;
    protected readonly int TimeoutSeconds;

    public BasePage(IWebDriver driver)
    {
        Driver = driver;
        TimeoutSeconds = ConfigLoader.GetExplicitWaitSeconds();
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(TimeoutSeconds));
    }

    /// <summary>
    /// Wait for element to be visible
    /// </summary>
    protected IWebElement WaitForElementVisible(By locator, string elementName)
    {
        try
        {
            Logger.Debug($"Waiting for element to be visible: {elementName}");
            return Wait.Until(d =>
            {
                var element = d.FindElement(locator);
                return element.Displayed ? element : null;
            }) ?? throw new TimeoutException($"Element not visible: {elementName}");
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.Error($"Element not visible within {TimeoutSeconds} seconds: {elementName}");
            throw new TimeoutException($"Element visibility timeout: {elementName}", ex);
        }
    }

    /// <summary>
    /// Wait for element to be clickable
    /// </summary>
    protected IWebElement WaitForElementClickable(By locator, string elementName)
    {
        try
        {
            Logger.Debug($"Waiting for element to be clickable: {elementName}");
            return Wait.Until(d =>
            {
                try
                {
                    var element = d.FindElement(locator);
                    return element.Enabled && element.Displayed ? element : null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            }) ?? throw new TimeoutException($"Element not clickable: {elementName}");
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.Error($"Element not clickable within {TimeoutSeconds} seconds: {elementName}");
            throw new TimeoutException($"Element clickability timeout: {elementName}", ex);
        }
    }

    /// <summary>
    /// Wait for element to be present
    /// </summary>
    protected IWebElement WaitForElementPresent(By locator, string elementName)
    {
        const int maxRetries = 4;
        const int delaySeconds = 3;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Logger.Debug($"Waiting for element to be present: {elementName} (Attempt {attempt}/{maxRetries})");
                return Wait.Until(d => d.FindElement(locator));
            }
            catch (WebDriverTimeoutException) when (attempt < maxRetries)
            {
                Logger.Warn($"Element not found on attempt {attempt}/{maxRetries}: {elementName}. Retrying in {delaySeconds} seconds...");
                Thread.Sleep(delaySeconds * 1000); // 3 second delay
            }
            catch (WebDriverTimeoutException ex) when (attempt == maxRetries)
            {
                Logger.Error($"Element not present after {maxRetries} attempts with {delaySeconds}s delays: {elementName}");
                throw new TimeoutException($"Element presence timeout after {maxRetries} retries: {elementName}", ex);
            }
        }

        // This line should never be reached, but required for compilation
        throw new TimeoutException($"Failed to find element: {elementName}");
    }

    /// <summary>
    /// Wait until the Advantage Online Shopping loader disappears
    /// </summary>
    protected void WaitForPageLoaderToDisappear()
    {
        try
        {
            Logger.Info("Waiting for the page loader to disappear");
            Wait.Until(d =>
            {
                try
                {
                    var loader = d.FindElements(By.CssSelector(".loader"));
                    if (loader.Count == 0)
                    {
                        return true;
                    }

                    var firstLoader = loader[0];
                    return !firstLoader.Displayed;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            });
            Logger.Info("Page loader disappeared");
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.Warn(ex, "Page loader did not disappear before timeout; continuing");
        }
    }
    
    /// <summary>
    /// Safe click with explicit wait
    /// </summary>
    protected void SafeClick(By locator, string elementName)
    {
        try
        {
            Logger.Debug($"Attempting to click on: {elementName}");
            var element = WaitForElementClickable(locator, elementName);
            element.Click();
            Logger.Info($"Successfully clicked on: {elementName}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to click on element: {elementName}");
            throw new InvalidOperationException($"Failed to click on {elementName}", ex);
        }
    }

    /// <summary>
    /// Safe input with clear and wait
    /// </summary>
    protected void SafeInput(By locator, string text, string elementName)
    {
        try
        {
            Logger.Debug($"Attempting to input text in: {elementName}");
            var element = WaitForElementVisible(locator, elementName);
            element.Clear();
            element.SendKeys(text);
            Logger.Info($"Successfully entered text in: {elementName}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to input text in element: {elementName}");
            throw new InvalidOperationException($"Failed to input in {elementName}", ex);
        }
    }

    /// <summary>
    /// Get element text with error handling
    /// </summary>
    protected string GetElementText(By locator, string elementName)
    {
        try
        {
            Logger.Debug($"Attempting to retrieve text from: {elementName}");
            var element = WaitForElementVisible(locator, elementName);
            var text = element.Text.Trim();
            Logger.Info($"Retrieved text from {elementName}: {text}");
            return text;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to retrieve text from element: {elementName}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Check if element is displayed
    /// </summary>
    protected bool IsElementDisplayed(By locator, string elementName)
    {
        try
        {
            var element = Driver.FindElement(locator);
            WaitForElementPresent(locator, elementName);
            bool isDisplayed = element.Displayed;
            Logger.Debug($"Element {elementName} is displayed: {isDisplayed}");
            return isDisplayed;
        }
        catch (NoSuchElementException)
        {
            Logger.Debug($"Element {elementName} is not displayed");
            return false;
        }
    }

    /// <summary>
    /// Navigate to URL
    /// </summary>
    protected void NavigateTo(string url)
    {
        try
        {
            Logger.Info($"Navigating to: {url}");
            Driver.Navigate().GoToUrl(url);
            Logger.Info($"Successfully navigated to: {url}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to navigate to URL: {url}");
            throw new InvalidOperationException($"Failed to navigate to {url}", ex);
        }
    }

    /// <summary>
    /// Find multiple elements by locator
    /// </summary>
    protected IReadOnlyList<IWebElement> FindElements(By locator)
    {
        try
        {
            return Driver.FindElements(locator);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to find elements");
            return new List<IWebElement>();
        }
    }

    /// <summary>
    /// Wait for number of elements
    /// </summary>
    protected void WaitForNumberOfElements(By locator, int count)
    {
        try
        {
            Wait.Until(d => d.FindElements(locator).Count > count - 1);
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.Error(ex, $"Expected {count} elements not found");
            throw new TimeoutException($"Element count timeout", ex);
        }
    }

    /// <summary>
    /// Wait for specified seconds
    /// </summary>
    protected void WaitSeconds(int seconds)
    {
        try
        {
            Logger.Debug($"Waiting for {seconds} seconds");
            System.Threading.Thread.Sleep(seconds * 1000);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Wait interrupted");
        }
    }
}