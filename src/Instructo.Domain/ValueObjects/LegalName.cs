using Instructo.Domain.Shared;

using static Instructo.Domain.ValueObjects.ResultHelperExtensions;
namespace Instructo.Domain.ValueObjects;

public readonly record struct LegalName
{
    public string Value { get; }
    private LegalName(string value) =>
        Value=value;

    public static LegalName Empty => new(string.Empty);

    public static Result<LegalName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Failure<LegalName>(value, "Company name cannot be empty");
        value=value.Trim();
        if(value.Length>100)
            return Failure<LegalName>(value, "Company name cannot be longer than 100 characters");
        if(value.FirstOrDefault().ToString().Equals(value.FirstOrDefault().ToString().ToLower()))
            return Failure<LegalName>(value, "Company name must start with a capital letter");

        return new LegalName(value);
    }
    public static LegalName Wrap(string value) => new(value);
    public static implicit operator string(LegalName value) => value.Value;

    public override string ToString()
    {
        return Value;
    }
}

public class ResultHelperExtensions
{
    public static Result<T> Failure<T>(string value, string message)
    {
        return Result<T>.Failure([new Error(message, $"{value} for {typeof(T).Name}")]);
    }
}

public readonly record struct SchoolName
{
    public string Value { get; }
    private SchoolName(string value) =>
        Value=value;

    public static SchoolName Empty => new(string.Empty);

    public static Result<SchoolName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<SchoolName>.Failure([new Error("School name cannot be empty", value)]);
        if(value.Length>100)
            return Result<SchoolName>.Failure([new Error("School name cannot be longer than 100 characters", value)]);
        return new SchoolName(value);
    }
    public static SchoolName Wrap(string value) => new(value);

    public static implicit operator string(SchoolName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

public readonly record struct Url
{
    public string Value { get; }
    private Url(string value) =>
        Value=value;

    public static Url Empty => new(string.Empty);

    public static Result<Url> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Url>.Failure([new Error("Url cannot be empty", value)]);
        if(value.Length>2000)
            return Result<Url>.Failure([new Error("Url cannot be longer than 2000 characters", value)]);
        return new Url(value);
    }
    public static Url Wrap(string value) => new(value);

    public static implicit operator string(Url value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

public readonly record struct WebsiteLinkName
{
    public string Value { get; }
    private WebsiteLinkName(string value) =>
        Value=value;

    public static WebsiteLinkName Empty => new(string.Empty);

    public static Result<WebsiteLinkName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<WebsiteLinkName>.Failure([new Error("Website link name cannot be empty", value)]);
        if(value.Length>100)
            return Result<WebsiteLinkName>.Failure([new Error("Website link name cannot be longer than 100 characters", value)]);
        return new WebsiteLinkName(value);
    }

    public static WebsiteLinkName Wrap(string value) => new(value);

    public static implicit operator string(WebsiteLinkName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

public readonly record struct FileName
{
    public string Value { get; }
    private FileName(string value) =>
        Value=value;

    public static FileName Empty => new(string.Empty);

    public static Result<FileName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<FileName>.Failure([new Error("File name cannot be empty", value)]);
        if(value.Length>100)
            return Result<FileName>.Failure([new Error("File name cannot be longer than 100 characters", value)]);
        return new FileName(value);
    }

    public static FileName Wrap(string value) => new(value);
    public static implicit operator string(FileName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

public readonly record struct ContentType
{
    public string Value { get; }
    private ContentType(string value) =>
        Value=value;

    public static ContentType Empty => new(string.Empty);

    public static Result<ContentType> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<ContentType>.Failure([new Error("Content type cannot be empty", value)]);
        if(value.Length>100)
            return Result<ContentType>.Failure([new Error("Content type cannot be longer than 100 characters", value)]);
        return new ContentType(value);
    }
    public static ContentType Wrap(string value) => new(value);

    public static implicit operator string(ContentType value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//Email Domain Type
public readonly record struct Email
{
    public string Value { get; }
    private Email(string value) =>
        Value=value;
    public static Email Empty => new(string.Empty);
    public static Result<Email> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure([new Error("Email cannot be empty", value)]);
        if(value.Length>100)
            return Result<Email>.Failure([new Error("Email cannot be longer than 100 characters", value)]);
        return new Email(value);
    }
    public static Email Wrap(string value) => new(value);
    public static implicit operator string(Email value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//Password
public readonly record struct Password
{
    public string Value { get; }
    private Password(string value) =>
        Value=value;
    public static Password Empty => new(string.Empty);
    public static Result<Password> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Password>.Failure([new Error("Password cannot be empty", value)]);
        if(value.Length>100)
            return Result<Password>.Failure([new Error("Password cannot be longer than 100 characters", value)]);
        return new Password(value);
    }
    public static Password Wrap(string value) => new(value);
    public static implicit operator string(Password value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//Person First name
public readonly record struct Name
{
    public string Value { get; }
    private Name(string value) =>
        Value=value;
    public static Name Empty => new(string.Empty);
    public static Result<Name> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Name>.Failure([new Error("Name cannot be empty", value)]);
        if(value.Length>100)
            return Result<Name>.Failure([new Error("Name cannot be longer than 100 characters", value)]);
        return new Name(value);
    }
    public static Name Wrap(string value) => new(value);
    public static implicit operator string(Name value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}
//Phone Number
public record PhoneNumber
{
    private PhoneNumber() { }
    public string? Name { get; private set; }
    public string Value { get; private set; } = string.Empty;
    private PhoneNumber(string value, string? name = null)
    {
        Value=value;
        Name=name;
    }

    public static PhoneNumber Empty => new(string.Empty);
    public static Result<PhoneNumber> Create(string value, string? name = null)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<PhoneNumber>.Failure([new Error("Phone number cannot be empty", value)]);
        if(value.Length>100)
            return Result<PhoneNumber>.Failure([new Error("Phone number cannot be longer than 100 characters", value)]);
        return new PhoneNumber(value, name);
    }
    public static PhoneNumber Wrap(string value) => new(value);
    public static implicit operator string(PhoneNumber value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//Address
public readonly record struct Address
{
    public string Value { get; }
    private Address(string value) =>
        Value=value;
    public static Address Empty => new(string.Empty);
    public static Result<Address> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<Address>.Failure([new Error("Address cannot be empty", value)]);
        if(value.Length>100)
            return Result<Address>.Failure([new Error("Address cannot be longer than 100 characters", value)]);
        return new Address(value);
    }
    public static Address Wrap(string value) => new(value);
    public static implicit operator string(Address value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//City
public readonly record struct City
{
    public string Value { get; }
    private City(string value) =>
        Value=value;
    public static City Empty => new(string.Empty);
    public static Result<City> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<City>.Failure([new Error("City cannot be empty", value)]);
        if(value.Length>100)
            return Result<City>.Failure([new Error("City cannot be longer than 100 characters", value)]);
        return new City(value);
    }
    public static City Wrap(string value) => new(value);
    public static implicit operator string(City value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//FilePath
public readonly record struct FilePath
{
    public string Value { get; }
    private FilePath(string value) =>
        Value=value;
    public static FilePath Empty => new(string.Empty);
    public static Result<FilePath> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<FilePath>.Failure([new Error("File path cannot be empty", value)]);
        if(value.Length>100)
            return Result<FilePath>.Failure([new Error("File path cannot be longer than 100 characters", value)]);
        return new FilePath(value);
    }
    public static FilePath Wrap(string value) => new(value);
    public static implicit operator string(FilePath value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}

//Company name
public readonly record struct CompanyName
{
    public string Value { get; }
    private CompanyName(string value) =>
        Value=value;
    public static CompanyName Empty => new(string.Empty);
    public static Result<CompanyName> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            return Result<CompanyName>.Failure([new Error("Company name cannot be empty", value)]);
        if(value.Length>100)
            return Result<CompanyName>.Failure([new Error("Company name cannot be longer than 100 characters", value)]);
        return new CompanyName(value);
    }
    public static CompanyName Wrap(string value) => new(value);
    public static implicit operator string(CompanyName value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
}