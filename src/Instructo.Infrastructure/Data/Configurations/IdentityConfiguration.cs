using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Configurations;
internal static class IdentityConfiguration
{
    public static void ConfigureIdentity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>().ToTable("Users").HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
