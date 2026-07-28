# Advantage Shop Test Automation

This project contains Selenium-based UI automation tests for the Advantage Online Shopping website using .NET and NUnit.

## 1. Project Setup

The test framework is built with:
- .NET 10
- NUnit 4
- Selenium WebDriver
- WebDriverManager
- NLog for logging
- Allure for test reporting

### Project Structure
- `AdvantageShopTestAutomation/` - main test project
- `Tests/` - test classes
- `Pages/` - page object model classes
- `Config/` - browser and configuration helpers
- `Utils/` - reusable utilities such as test data generation and screenshots
- `.runsettings/` - test run configuration files

## 2. Project Requirements

Before running the tests, make sure you have the following installed:
- .NET SDK 10.0 or later
- Google Chrome or Mozilla Firefox installed
- Internet access to reach the target site

## 3. How to Install

1. Clone the repository:
   ```bash
   git clone https://github.com/santoshmohan/AdvantageShopTestAutomation.git
   cd AdvantageShopTestAutomation
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

## 4. How to Execute the Project

Run the test suite with the development run settings:

```bash
cd AdvantageShopTestAutomation
DOTNET_CLI_HOME="$TMPDIR" dotnet test --settings .runsettings/Development.runsettings --logger "console;verbosity=detailed"
```

### Run with a different environment
You can also use the production run settings:

```bash
DOTNET_CLI_HOME="$TMPDIR" dotnet test --settings .runsettings/Production.runsettings --logger "console;verbosity=detailed"
```


## Notes
- Browser selection and timeout values can be controlled through the run settings files.
- Logs are written using NLog and can be found in the configured log output location.
