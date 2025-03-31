using Bogus;

using Instructo.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace Instructo.Tests.Common;

public class UserBuilder
{
    private static readonly Faker _faker = new Faker("ro");
    private string _id = _faker.Random.Guid().ToString();
    private string _firstName = _faker.Person.FirstName;
    private string _lastName = _faker.Person.LastName;
    private string _email = _faker.Person.Email;
    private string _password = _faker.Internet.Password(8, false, null, "A1!"); // More realistic password matching requirements
    private string _phoneNumber = _faker.Phone.PhoneNumber();
    private List<string> _roles = new List<string>();

    public UserBuilder WithId(string id)
    {
        _id=id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email=email;
        return this;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName=firstName;
        return this;
    }

    public UserBuilder WithLastName(string lastName)
    {
        _lastName=lastName;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password=password;
        return this;
    }

    public UserBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber=phoneNumber;
        return this;
    }

    public UserBuilder WithRole(string role)
    {
        _roles.Add(role);
        return this;
    }

    // Builds just the entity without storing in DB
    public ApplicationUser Build()
    {
        return new ApplicationUser
        {
            Id=_id,
            Email=_email,
            FirstName=_firstName,
            LastName=_lastName,
            PhoneNumber=_phoneNumber,
            UserName=_email, // Identity requires a username
            EmailConfirmed=true // To simplify testing
        };
    }

    // Creates user in the actual identity system
    public async Task<ApplicationUser> CreateAsync(UserManager<ApplicationUser> userManager)
    {
        var user = Build();
        await userManager.CreateAsync(user, _password);

        foreach(var role in _roles)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    public static List<ApplicationUser> CreateDefaultUsers(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new UserBuilder()
                .WithEmail($"user{i}@example.com")
                .WithFirstName($"First{i}")
                .WithLastName($"Last{i}")
                .WithPassword("Password123!")
                .Build())
            .ToList();
    }

    // Creates a student with appropriate defaults
    public static UserBuilder CreateStudent()
    {
        return new UserBuilder().WithRole("Student");
    }

    // Creates an instructor with appropriate defaults
    public static UserBuilder CreateInstructor()
    {
        return new UserBuilder().WithRole("Instructor");
    }
}
