using Bogus;

using Instructo.Domain.Entities;

namespace Instructo.UnitTests.Application.Users.Common;

public class UserBuilder
{
    private static readonly Faker _faker = new Faker("ro");
    private string _id = _faker.UniqueIndex.ToString();
    private string _email = _faker.Person.Email;
    private string _name = _faker.Person.FirstName;
    private string _password = _faker.Internet.Password();
    private string _phoneNumber = _faker.Phone.PhoneNumber();
    // Add other properties

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

    public UserBuilder WithName(string name)
    {
        _name=name;
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
    // Add other with methods

    public ApplicationUser Build()
    {
        return new ApplicationUser
        {
            Id=_id,
            Email=_email,
            FirstName=_name,
            PasswordHash=_password,
            PhoneNumber=_phoneNumber
        };
    }

    public static List<ApplicationUser> CreateDefaultUsers(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => new UserBuilder()
                .WithEmail($"user{i}@example.com")
                .WithName($"User {i}")
                .WithPassword("password")
                .Build())
            .ToList();
    }
}
