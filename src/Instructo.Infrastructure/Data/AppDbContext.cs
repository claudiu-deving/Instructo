using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Entities;
using Instructo.Infrastructure.Data.Configurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Instructo.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for your entity models
    public DbSet<School> Schools { get; set; }
    public DbSet<WebsiteLink> WebsiteLinks { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Console.WriteLine("Creating");
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new SchoolsConfiguration());
        modelBuilder.ApplyConfiguration(new WebsiteLinksConfiguration());
        modelBuilder.ApplyConfiguration(new ImagesConfiguration());

        modelBuilder.Entity<ApplicationUser>().ToTable("Users").HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

    }
}
