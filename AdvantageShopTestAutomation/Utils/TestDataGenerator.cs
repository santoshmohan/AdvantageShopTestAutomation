using Bogus;
using NLog;
using AdvantageShopTestAutomation.Models;

namespace AdvantageShopTestAutomation.Utils;


public class TestDataGenerator
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private static readonly Faker Faker = new Faker();
    private static readonly String userNamePrefix = "Auto_";
    public static string GenerateUniqueUsername()
    {
        var username = $"{userNamePrefix}{Faker.Internet.UserName()}";
        // Truncate username to a maximum of 15 characters to meet potential field length constraints
        if (username.Length > 15)
        {
            username = username.Substring(0, 15);
        }
        Logger.Debug($"Generated unique username: {username}");
        return username;
    }

    public static string GenerateUniqueEmail()
    {
        var email = $"{Faker.Internet.UserName()}@example.com";
        Logger.Debug($"Generated unique email: {email}");
        return email;
    }
    
    public static string GenerateSecurePassword()
    {
        var password = "PassW0rd@1";
        Logger.Debug($"Generated secure password: {password}");
        return password;
    }

    public static string GenerateFirstName() => Faker.Name.FirstName();

    public static string GenerateLastName() => Faker.Name.LastName();

    public static string GenerateFullName() => Faker.Name.FullName();

    public static string GenerateStreetAddress() => Faker.Address.StreetAddress();

    public static string GenerateCity() => Faker.Address.City();

    public static string GenerateState() => Faker.Address.State();

    public static string GeneratePostalCode() => Faker.Address.ZipCode();

    public static string GenerateCountry() => Faker.Address.Country();

    public static string GeneratePhoneNumber() => Faker.Phone.PhoneNumber();

    public static string GenerateCompanyName() => Faker.Company.CompanyName();

    public static string GenerateCompleteAddress() =>
        $"{GenerateStreetAddress()}, {GenerateCity()}, {GenerateState()} {GeneratePostalCode()}, {GenerateCountry()}";


    public static void LogTestData(string label, string value)
    {
        Logger.Info($"{label}: {value}");
    }
}