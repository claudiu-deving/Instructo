using Domain.Entities;
using Domain.Entities.SchoolEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public interface IAppDbContext
{
    DbSet<School> Schools { get; set; }
    DbSet<WebsiteLink> WebsiteLinks { get; set; }
    DbSet<Image> Images { get; set; }
    DbSet<ArrCertificate> CertificateTypes { get; set; }
    DbSet<VehicleCategory> Categories { get; set; }
    DbSet<ApplicationUser> Users { get; set; }
    DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
    DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
    DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
    DbSet<ApplicationRole> Roles { get; set; }
    DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
    int SaveChanges(bool acceptAllChangesOnSuccess);
    Task<int> SaveChangesAsync();
}