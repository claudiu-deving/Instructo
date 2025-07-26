using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Instructo.IntegrationTests.Infrastructure;

public class AuthenticationHelper(IServiceProvider serviceProvider)
{
    public string CreateJwtToken(ApplicationUser user, string roleName)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var jwtSettings = configuration.GetSection("JwtSettings");

        // Use "Secret" instead of "SecretKey" to match the API configuration
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]??"YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForTesting!");
        var issuer = jwtSettings["Issuer"]??"Instructo";
        var audience = jwtSettings["Audience"]??"InstructoUsers";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, roleName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject=new ClaimsIdentity(claims),
            Expires=DateTime.UtcNow.AddHours(1),
            Issuer=issuer,
            Audience=audience,
            SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization=new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }


    public async Task<ApplicationUser> CreateTestUserAsync(string role = "User")
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if(role==ApplicationRole.IronMan.Name!)
        {
            var ironManUser = await userManager.GetUserAsync(
                 new ClaimsPrincipal(new ClaimsIdentity(
                 [
                     new(ClaimTypes.NameIdentifier,"8fb090d2-ad97-41d0-86ce-08ddc4a5a731"),
                    new(ClaimTypes.Role, ApplicationRole.IronMan.Name!)
                 ], "TestAuthentication"))
             );
            return ironManUser??throw new InvalidOperationException("Test user with IronMan role not found");
        }
        var user = new ApplicationUser
        {
            Id=Guid.NewGuid(),
            Email=$"test-{Guid.NewGuid():N}@example.com",
            FirstName="Test",
            LastName="User",
            UserName=$"test-{Guid.NewGuid():N}@example.com",
            EmailConfirmed=true,
            IsActive=true
        }??throw new InvalidOperationException("Failed to create test user: user is null");

        user.NormalizedEmail=user.Email?.ToUpperInvariant();
        user.NormalizedUserName=user.UserName?.ToUpperInvariant();

        var result = await userManager.CreateAsync(user, "Password123!");
        if(!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        // Ensure role exists
        if(!await roleManager.RoleExistsAsync(role))
        {
            var applicationRole = ApplicationRole.AllRoles.FirstOrDefault(r => r.Name==role)
                ??new ApplicationRole { Name=role, Description=$"Test {role} role" };
            await roleManager.CreateAsync(applicationRole);
        }

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}