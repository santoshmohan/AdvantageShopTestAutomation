using OpenQA.Selenium;

namespace AdvantageShopTestAutomation.Pages;

/// <summary>
/// Login/Register modal page object
/// </summary>
public class LoginRegisterModal : BasePage
{
    // Locators using By class
    private readonly By CreateNewAccountButton = By.CssSelector("a.create-new-account");
    private readonly By UserName = By.CssSelector("login-modal input[name='username']");
    private readonly By Password = By.CssSelector("login-modal input[name='password']");

    public LoginRegisterModal(IWebDriver driver) : base(driver)
    {
        Logger.Info("LoginRegisterModal initialized");
    }

    /// <summary>
    /// Click on 'CREATE NEW ACCOUNT' button
    /// </summary>
    public RegisterPage ClickCreateNewAccountButton()
    {
        Logger.Info("Clicking on 'CREATE NEW ACCOUNT' button");
        SafeClick(CreateNewAccountButton, "Create New Account Button");
        WaitForPageLoaderToDisappear();
        Thread.Sleep(1000); // Wait for register page to load
        return new RegisterPage(Driver);
    }

    /// <summary>
    /// Verify create new account button is visible
    /// </summary>
    public bool IsCreateNewAccountButtonVisible() =>
        IsElementDisplayed(CreateNewAccountButton, "Create New Account Button");
    
    /// <summary>
    /// Verify user name field is visible
    /// </summary>
    public bool IsUserNameFieldVisible() =>
        IsElementDisplayed(UserName, "User Name Field");

    /// <summary>
    /// Verify password field is visible
    /// </summary>
    public bool IsPasswordFieldVisible() =>
        IsElementDisplayed(Password, "Password Field");
}