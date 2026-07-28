using NUnit.Framework;
using NLog;
using AdvantageShopTestAutomation.Tests.Base;
using AdvantageShopTestAutomation.Pages;
using AdvantageShopTestAutomation.Utils;

namespace AdvantageShopTestAutomation.Tests;

/// <summary>
/// Test cases for login and registration functionality
/// </summary>
[TestFixture]
[Category("LoginRegister")]
public class LoginRegisterTests : BaseTest
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    [Test]
    [Category("Validation")]
    [Description("LOG001 - Mandatory field error messages display & clear")]
    public void TestMandatoryFieldValidation()
    {
        try
        {
            // Step 1: Navigate to home page
            Logger.Info("TEST CASE: LOG001 - Mandatory field error messages display & clear");
            var homePage = new HomePage(Driver);
            homePage.LoadHomePage();
            Assert.That(homePage.IsUserIconVisible(), Is.True, "User icon should be visible on home page");
            Logger.Info("STEP 1 PASSED: Home page loaded successfully");

            // Step 2: Click user icon to open login/register modal
            var loginModal = homePage.ClickUserIcon();
        
            Assert.That(loginModal.IsPasswordFieldVisible(), Is.True,
                "Password field should be visible in modal");
            Logger.Info("STEP 2 PASSED: Login/Register modal opened successfully");

            // Step 3: Click 'CREATE NEW ACCOUNT' button
            var registerPage = loginModal.ClickCreateNewAccountButton();
            Logger.Info("STEP 3 PASSED: Register page loaded successfully");

            // Step 4: Interact with form fields to trigger validation errors
            Logger.Info("STEP 4: Interacting with form fields to trigger validation");
            Assert.That(registerPage.IsCreateAccountHeadingVisible(), Is.True, "Create Account heading should be visible");

            registerPage.InteractWithUsernameField();
            WaitSeconds(1);
            int errorCountAfterUsername = registerPage.GetErrorMessageCount();
            Logger.Info($"Error count after username field interaction: {errorCountAfterUsername}");
            Assert.That(errorCountAfterUsername, Is.GreaterThan(0),
                "Errors should display after username field interaction");

            registerPage.InteractWithEmailField();
            WaitSeconds(1);
            int errorCountAfterEmail = registerPage.GetErrorMessageCount();
            Logger.Info($"Error count after email field interaction: {errorCountAfterEmail}");
            Assert.That(errorCountAfterEmail, Is.GreaterThan(0),
                "Errors should display after email field interaction");

            registerPage.InteractWithPasswordField();
            WaitSeconds(1);
            int errorCountAfterPassword = registerPage.GetErrorMessageCount();
            Logger.Info($"Error count after password field interaction: {errorCountAfterPassword}");
            Assert.That(errorCountAfterPassword, Is.GreaterThan(0),
                "Errors should display after password field interaction");

            registerPage.InteractWithConfirmPasswordField();
            WaitSeconds(1);
            int totalErrors = registerPage.GetErrorMessageCount();
            Logger.Info($"Total error count after all field interactions: {totalErrors}");
            Assert.That(totalErrors, Is.GreaterThan(0), "Multiple validation errors should be displayed");
            Logger.Info("STEP 4 PASSED: Validation errors displayed correctly");

            // Step 5: Verify error messages contain correct content
            Logger.Info("STEP 5: Verifying error message content");
            bool usernameErrorFound = registerPage.IsErrorMessageDisplayed("Username") ||
                                      registerPage.IsErrorMessageDisplayed("required");
            Assert.That(usernameErrorFound, Is.True, "Username error message should be displayed");

            bool emailErrorFound = registerPage.IsErrorMessageDisplayed("Email") ||
                                   registerPage.IsErrorMessageDisplayed("required");
            Assert.That(emailErrorFound, Is.True, "Email error message should be displayed");

            bool passwordErrorFound = registerPage.IsErrorMessageDisplayed("Password") ||
                                      registerPage.IsErrorMessageDisplayed("required");
            Assert.That(passwordErrorFound, Is.True, "Password error message should be displayed");

            Logger.Info("STEP 5 PASSED: All error messages contain correct content");

            // Step 6: Enter valid data using test data generator
            Logger.Info("STEP 6: Generating test data and entering valid data in all fields");

            string username = TestDataGenerator.GenerateUniqueUsername();
            string email = TestDataGenerator.GenerateUniqueEmail();
            string password = TestDataGenerator.GenerateSecurePassword();
            string confirmPassword = password;

            Logger.Info("Generated Test Data:");
            Logger.Info($"  Username: {username}");
            Logger.Info($"  Email: {email}");
            Logger.Info($"  First Name: {TestDataGenerator.GenerateFirstName()}");
            Logger.Info($"  Last Name: {TestDataGenerator.GenerateLastName()}");
            Logger.Info($"  Full Address: {TestDataGenerator.GenerateCompleteAddress()}");

            registerPage.FillRegistrationForm(username, email, password, confirmPassword);
            WaitSeconds(2);
            Logger.Info("STEP 6 PASSED: Valid data entered successfully");

            // Step 7: Verify all errors are cleared
            Logger.Info("STEP 7: Verifying all errors are cleared");
            bool allErrorsCleared = registerPage.AreAllErrorsCleared();
            Assert.That(allErrorsCleared, Is.True,
                "All validation errors should be cleared after entering valid data");
            Logger.Info("STEP 7 PASSED: All validation errors cleared successfully");

            Logger.Info("TEST CASE LOG001 PASSED SUCCESSFULLY");
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Test failed with exception");
            throw;
        }
    }
}