using Microsoft.AspNetCore.Identity;


namespace Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
    public DateTime Created { get; set; }
    // Other role properties




    public static ApplicationRole IronMan =>
        new ApplicationRole() { Name="IronMan", Description="I AM IRONMAN" };
    public static ApplicationRole Owner =>
        new ApplicationRole() { Name="Owner", Description="School owner" };
    public static ApplicationRole Admin =>
        new ApplicationRole() { Name="Admin", Description="School administrator, a bit less rights than an Owner" };
    public static ApplicationRole Instructor =>
        new ApplicationRole() { Name="Instructor", Description="Instructor, can schedule lesssons and manage students" };
    public static ApplicationRole Student =>
        new ApplicationRole() { Name="Student", Description="Student, can view lessons and grades" };
    public static ApplicationRole User =>
        new ApplicationRole() { Name="User", Description="Regular user, can view public information" };

    public static ApplicationRole[] AllRoles =>
        [
            IronMan,
            Owner,
            Admin,
            Instructor,
            Student,
            User
        ];
}