using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [NotMapped] public virtual ApplicationRole? Role { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public virtual School? School { get; set; }

    public override bool Equals(object? obj)
    {
        if(obj==null||GetType()!=obj.GetType())
            return false;
        var user = (ApplicationUser)obj;
        return user.Id==Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}