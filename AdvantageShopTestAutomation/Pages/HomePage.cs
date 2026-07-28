using OpenQA.Selenium;
using AdvantageShopTestAutomation.Config;

namespace AdvantageShopTestAutomation.Pages;

/// <summary>
/// Home page object for Advantage Online Shopping website
/// </summary>
public class HomePage : BasePage
{
    private readonly By UserIconButton = By.CssSelector("a#menuUserLink");
    public HomePage(IWebDriver driver) : base(driver)
    {
        Logger.Info("HomePage initialized");
    }

    /// <summary>
    /// Load home page
    /// </summary>
    public void LoadHomePage()
    {
        string baseUrl = ConfigLoader.GetBaseUrl();
        NavigateTo(baseUrl);
        WaitForPageLoaderToDisappear();
        Thread.Sleep(8000); // Wait for page to fully load
        Logger.Info($"Home page loaded from: {baseUrl}");
    }

    /// <summary>
    /// Click on user icon to open login/register modal
    /// </summary>
    public LoginRegisterModal ClickUserIcon()
    {
        Logger.Info("Clicking on user icon to open login/register modal");
        WaitForPageLoaderToDisappear();
        SafeClick(UserIconButton, "User Icon");
        WaitForElementPresent(By.CssSelector("login-modal .closeBtn"), "Close Modal");
        Thread.Sleep(1000); // Wait for modal to fully load
        return new LoginRegisterModal(Driver);
    }

    /// <summary>
    /// Verify user icon is visible
    /// </summary>
    public bool IsUserIconVisible() {
        WaitForElementPresent(UserIconButton, "User Icon");
        return IsElementDisplayed(UserIconButton, "User Icon"); }
}