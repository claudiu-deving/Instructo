using Instructo.Domain.ValueObjects;

using Microsoft.AspNetCore.Identity;


namespace Instructo.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    public DateTime Created { get; set; }
    // Other role properties
}