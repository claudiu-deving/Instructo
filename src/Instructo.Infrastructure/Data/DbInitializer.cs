using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bogus;

using Instructo.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Instructo.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        string[] roleNames = { "Admin", "SchoolOwner", "Instructor", "Student" };


        int count = 2000;
        // Generate fake users with Bogus
        var faker = new Faker<ApplicationUser>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName).ToLowerInvariant())
            .RuleFor(u => u.UserName, (f, u) => u.Email)
            .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool(0.9f))  // 90% have confirmed emails
            .RuleFor(u => u.Created, f => f.Date.Past(2))
            .RuleFor(u => u.LastLogin, f => f.Random.Bool(0.7f) ? f.Date.Recent(30) : null)  // 70% have logged in recently
            .RuleFor(u => u.IsActive, f => f.Random.Bool(0.85f));  // 85% are active

        // Generate users
        var users = faker.Generate(count);
        int created = 0;
        var fakerGenerator = new Faker();
        foreach(var user in users)
        {
            // Skip if email already exists
            if(await userManager.FindByEmailAsync(user.Email)!=null)
                continue;

            var result = await userManager.CreateAsync(user, "Password123!");
            if(result.Succeeded)
            {
                created++;

                // Assign a random role
                var role = fakerGenerator.Random.ArrayElement(roleNames);
                await userManager.AddToRoleAsync(user, role);

                // Report progress every 100 users
                if(created%100==0)
                {
                    Console.WriteLine($"Created {created} users");
                }
            }
        }
    }
}
