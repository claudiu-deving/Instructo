using Microsoft.AspNetCore.Identity;


namespace Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    public DateTime Created { get; set; }
    // Other role properties
    
    public static ApplicationRole IronMan=>
        new  ApplicationRole(){Name = "IronMan", Description = "I AM IRONMAN"};
    public static ApplicationRole Owner=>
        new  ApplicationRole(){Name = "Owner", Description = "School owner"};
    public static ApplicationRole Admin=>
        new  ApplicationRole(){Name = "Admin", Description = "School administrator, a bit less rights than an Owner"};
}