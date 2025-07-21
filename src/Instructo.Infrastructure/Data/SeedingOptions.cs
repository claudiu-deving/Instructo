namespace Infrastructure.Data;

/// <summary>
/// Configuration options for database seeding
/// </summary>
public class SeedingOptions
{
    public const string Section = "Seeding";
    
    /// <summary>
    /// Whether to enable seeding of schools
    /// </summary>
    public bool EnableSchoolSeeding { get; set; } = true;
    
    /// <summary>
    /// Minimum number of schools to ensure exist in the database
    /// </summary>
    public int MinimumSchoolCount { get; set; } = 20;
    
    /// <summary>
    /// Whether to use hardcoded SQL script for test data (legacy mode)
    /// </summary>
    public bool UseLegacyTestData { get; set; } = false;
    
    /// <summary>
    /// Whether to force re-seeding even if minimum count is met
    /// </summary>
    public bool ForceReseed { get; set; } = false;
}