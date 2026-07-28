using OpenQA.Selenium;

namespace AdvantageShopTestAutomation.Pages;

/// <summary>
/// Registration page object
/// </summary>
public class RegisterPage : BasePage
{
    // Form field locators using By class
    private readonly By CreateAccountHeading = By.XPath("//h3[contains(text(), 'CREATE ACCOUNT')]");
    private readonly By UsernameField = By.Name("usernameRegisterPage");
    private readonly By EmailField = By.Name("emailRegisterPage");
    private readonly By PasswordField = By.Name("passwordRegisterPage");
    private readonly By ConfirmPasswordField = By.Name("confirm_passwordRegisterPage");

    // Error message locators
    private readonly By AllErrorMessages = By.CssSelector("div#form label.invalid");

    // Register button locator
    private readonly By RegisterButton = By.XPath("//button[contains(text(), 'Register')] | //button[@id='registerBtn']");

    public RegisterPage(IWebDriver driver) : base(driver)
    {
        Logger.Info("RegisterPage initialized");
    }

    /// <summary>
    /// Click into username field and then out (to trigger validation)
    /// </summary>
    public void InteractWithUsernameField()
    {
        Logger.Info("Interacting with Username field");
        SafeClick(UsernameField, "Username Field");
        SafeClick(EmailField, "Email Field");
        Logger.Info("Username field interaction completed");
    }

    /// <summary>
    /// Click into email field and then out (to trigger validation)
    /// </summary>
    public void InteractWithEmailField()
    {
        Logger.Info("Interacting with Email field");
        SafeClick(EmailField, "Email Field");
        SafeClick(PasswordField, "Password Field");
        Logger.Info("Email field interaction completed");
    }

    /// <summary>
    /// Click into password field and then out (to trigger validation)
    /// </summary>
    public void InteractWithPasswordField()
    {
        Logger.Info("Interacting with Password field");
        SafeClick(PasswordField, "Password Field");
        SafeClick(ConfirmPasswordField, "Confirm Password Field");
        Logger.Info("Password field interaction completed");
    }

    /// <summary>
    /// Click into confirm password field and then out (to trigger validation)
    /// </summary>
    public void InteractWithConfirmPasswordField()
    {
        Logger.Info("Interacting with Confirm Password field");
        SafeClick(ConfirmPasswordField, "Confirm Password Field");
        SafeClick(UsernameField, "Username Field");
        Logger.Info("Confirm Password field interaction completed");
    }

    /// <summary>
    /// Get all error messages currently displayed
    /// </summary>
    public IReadOnlyList<IWebElement> GetErrorMessages()
    {
        Logger.Info("Retrieving error messages");
        return FindElements(AllErrorMessages);
    }

    /// <summary>
    /// Get error message count
    /// </summary>
    public int GetErrorMessageCount()
    {
        int count = GetErrorMessages().Count;
        Logger.Info($"Error message count: {count}");
        return count;
    }

    /// <summary>
    /// Get specific error message text by index
    /// </summary>
    public string GetErrorMessageText(int index)
    {
        try
        {
            var errors = GetErrorMessages();
            if (index < errors.Count)
            {
                string errorText = errors[index].Text;
                Logger.Info($"Error message at index {index}: {errorText}");
                return errorText;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to retrieve error message at index: {index}");
        }
        return string.Empty;
    }

    /// <summary>
    /// Check if a specific error message is displayed
    /// </summary>
    public bool IsErrorMessageDisplayed(string expectedErrorText)
    {
        Logger.Info($"Checking if error message is displayed: {expectedErrorText}");
        var errors = GetErrorMessages();
        foreach (var error in errors)
        {
            if (error.Text.Contains(expectedErrorText))
            {
                Logger.Info($"Error message found: {expectedErrorText}");
                return true;
            }
        }
        Logger.Warn($"Error message not found: {expectedErrorText}");
        return false;
    }

    /// <summary>
    /// Enter username
    /// </summary>
    public void EnterUsername(string username)
    {
        Logger.Info("Entering username");
        SafeInput(UsernameField, username, "Username Field");
    }

    /// <summary>
    /// Enter email
    /// </summary>
    public void EnterEmail(string email)
    {
        Logger.Info("Entering email");
        SafeInput(EmailField, email, "Email Field");
    }

    /// <summary>
    /// Enter password
    /// </summary>
    public void EnterPassword(string password)
    {
        Logger.Info("Entering password");
        SafeInput(PasswordField, password, "Password Field");
    }

    /// <summary>
    /// Enter confirm password
    /// </summary>
    public void EnterConfirmPassword(string confirmPassword)
    {
        Logger.Info("Entering confirm password");
        SafeInput(ConfirmPasswordField, confirmPassword, "Confirm Password Field");
    }

    /// <summary>
    /// Fill complete registration form at once
    /// </summary>
    public void FillRegistrationForm(string username, string email, string password, string confirmPassword)
    {
        Logger.Info("Filling complete registration form");
        EnterUsername(username);
        EnterEmail(email);
        EnterPassword(password);
        EnterConfirmPassword(confirmPassword);
    }

    /// <summary>
    /// Click register button
    /// </summary>
    public void ClickRegisterButton()
    {
        Logger.Info("Clicking Register button");
        SafeClick(RegisterButton, "Register Button");
    }

    /// <summary>
    /// Check if all error messages are cleared
    /// </summary>
    public bool AreAllErrorsCleared()
    {
        int errorCount = GetErrorMessageCount();
        bool cleared = errorCount == 0;
        Logger.Info($"All errors cleared: {cleared} (Error count: {errorCount})");
        return cleared;
    }

    public bool IsCreateAccountHeadingVisible() =>
        IsElementDisplayed(CreateAccountHeading, "Create Account Heading");
}