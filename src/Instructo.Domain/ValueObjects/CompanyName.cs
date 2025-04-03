namespace Instructo.Domain.ValueObjects;

public readonly record struct CompanyName
{
    public string Name { get; }
    public CompanyName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));
        if(name.Length>100)
            throw new ArgumentException("Company name cannot be longer than 100 characters", nameof(name));
        Name=name;
    }

    public static implicit operator CompanyName(string name) => new CompanyName(name);
    public static implicit operator string(CompanyName name) => name.Name;
}
public readonly record struct SchoolName
{
    public string Name { get; }
    public SchoolName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("School name cannot be empty", nameof(name));
        if(name.Length>100)
            throw new ArgumentException("School name cannot be longer than 100 characters", nameof(name));
        Name=name;
    }

    public static implicit operator SchoolName(string name) => new SchoolName(name);
    public static implicit operator string(SchoolName name) => name.Name;
}

public readonly record struct Url
{
    public string Value { get; }
    public Url(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Url cannot be empty", nameof(value));
        if(value.Length>2000)
            throw new ArgumentException("Url cannot be longer than 2000 characters", nameof(value));
        Value=value;
    }
    public static implicit operator Url(string value) => new Url(value);
    public static implicit operator string(Url value) => value.Value;
}

public readonly record struct WebsiteLinkName
{
    public string Name { get; }
    public WebsiteLinkName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Website link name cannot be empty", nameof(name));
        if(name.Length>100)
            throw new ArgumentException("Website link name cannot be longer than 100 characters", nameof(name));
        Name=name;
    }
    public static implicit operator WebsiteLinkName(string name) => new WebsiteLinkName(name);
    public static implicit operator string(WebsiteLinkName name) => name.Name;
}

public readonly record struct FileName
{
    public string Name { get; }
    public FileName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("File name cannot be empty", nameof(name));
        if(name.Length>100)
            throw new ArgumentException("File name cannot be longer than 100 characters", nameof(name));
        Name=name;
    }
    public static implicit operator FileName(string name) => new FileName(name);
    public static implicit operator string(FileName name) => name.Name;
}

public readonly record struct ContentType
{
    public string Name { get; }
    public ContentType(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Content type cannot be empty", nameof(name));
        if(name.Length>100)
            throw new ArgumentException("Content type cannot be longer than 100 characters", nameof(name));
        Name=name;
    }
    public static implicit operator ContentType(string name) => new ContentType(name);
    public static implicit operator string(ContentType name) => name.Name;
}