using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Instructo.IntegrationTests.Data.Repositories.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Instructo.IntegrationTests;

public class AuthenticationHelper
{
    private readonly IntegrationTestFixture _fixture;

    public AuthenticationHelper(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task<string> GetJwtTokenAsync(string email, string role = "Student", string? userId = null)
    {
        using var scope = _fixture.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Create user if doesn't exist
            user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = "Test",
                LastName = "User",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, role);
        }

        return GenerateJwtToken(user, role, userId);
    }

    public async Task<string> GetIronManTokenAsync()
    {
        return await GetJwtTokenAsync("claudiu.c.strugar@gmail.com", "IronMan", "d9d42c1e-024a-489f-61f7-08dd73a823b2");
    }

    public async Task<string> GetAdminTokenAsync()
    {
        return await GetJwtTokenAsync("admin@test.com", "Admin");
    }

    public async Task<string> GetOwnerTokenAsync()
    {
        return await GetJwtTokenAsync("owner@test.com", "Owner");
    }

    private string GenerateJwtToken(ApplicationUser user, string role, string? userId = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // Use the same key that your application uses - you might want to get this from configuration
        var key = Encoding.ASCII.GetBytes("your-super-secret-key-with-at-least-32-characters");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId ?? user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = "instructo-client",
            Issuer = "instructo-api"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}