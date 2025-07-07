using Domain.Shared;
namespace Domain.ValueObjects;

public readonly partial record struct Slug
{
    public string Value { get; }
    private Slug(string value) =>
        Value=value;
    public static Slug Empty => new(LegalName.Empty);
    public static Slug Create(LegalName legalName)
    {
        var safe = CreateUrlSafeSlug(legalName.Value);

        return new Slug(safe);
    }
    public static Slug Wrap(string value) => new(value);
    public static implicit operator string(Slug value) => value.Value;
    public override string ToString()
    {
        return Value;
    }
    private static string CreateUrlSafeSlug(string input)
    {
        if(string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var slug = input.Trim().ToLowerInvariant();

        // Replace spaces and non-alphanumeric characters with hyphens
        slug=ReplaceWithHyphens().Replace(slug, "-");

        // Remove multiple consecutive hyphens
        slug=MultipleConsecutiveHyphens().Replace(slug, "-");

        // Remove leading and trailing hyphens
        slug=slug.Trim('-');

        return slug;
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"[^a-z0-9]")]
    private static partial System.Text.RegularExpressions.Regex ReplaceWithHyphens();
    [System.Text.RegularExpressions.GeneratedRegex(",")]
    private static partial System.Text.RegularExpressions.Regex MultipleConsecutiveHyphens();
}