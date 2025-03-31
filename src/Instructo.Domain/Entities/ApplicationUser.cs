using Microsoft.AspNetCore.Identity;


namespace Instructo.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }

    public override bool Equals(object obj)
    {
        if(obj==null||GetType()!=obj.GetType())
        {
            return false;
        }
        var user = (ApplicationUser)obj;
        return user.Id==Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
