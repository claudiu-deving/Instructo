using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Instructo.Domain.Dtos.User;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;

namespace Instructo.Infrastructure.Identity;
public class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtSettings> jwtSettings) : IIdentityService
{
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid id)
    {
        return await userManager.FindByIdAsync(id.ToString());
    }
    public async Task<Result<string>> CheckPasswordSignInAsync(ApplicationUser user, string password)
    {
        if((await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)).Succeeded)
        {
            user.AccessFailedCount++;
            await userManager.UpdateAsync(user);
            return Result<string>.Success("User signed in");
        }
        else
        {
            return Result<string>.Failure([new Error("SignIn", "User sign in failed")]);
        }

    }
    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);

        if(user.UserName is null)
        {
            throw new ArgumentNullException(nameof(user.UserName));
        }
        if(user.Email is null)
        {
            throw new ArgumentNullException(nameof(user.Email));
        }

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        foreach(var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(jwtSettings.Value.ExpiryDays);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<Result<string>> CreateAsync(ApplicationUser user, string password)
    {
        var registrationRequest = await userManager.CreateAsync(user, password);
        if(registrationRequest.Succeeded)
        {
            return Result<string>.Success("User created");
        }
        else
        {
            return Result<string>.Failure(registrationRequest.Errors.Select(x => new Error(x.Code, x.Description)).ToArray());
        }
    }
    public async Task<Result<string>> AddToRoleAsync(ApplicationUser user, string role)
    {

        if((await userManager.AddToRoleAsync(user, role)).Succeeded)
        {
            return Result<string>.Success("Role added");
        }
        else
        {
            return Result<string>.Failure([new Error("Add-Role", "Role addition failed")]);
        }
    }
    public async Task<Result<string>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        if((await userManager.ChangePasswordAsync(user, currentPassword, newPassword)).Succeeded)
        {
            return Result<string>.Success("Password changed");
        }
        else
        {
            return Result<string>.Failure([new Error("Change-Password", "Password change failed")]);
        }
    }
    public Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        return Task.FromResult(Result<string>.Success("Email sent"));
    }
    public async Task<Result<string>> UpdateAsync(ApplicationUser user)
    {
        if((await userManager.UpdateAsync(user)).Succeeded)
        {
            return Result<string>.Success("User updated");
        }
        else
        {
            return Result<string>.Failure([new Error("Update-User", "User update failed")]);
        }
    }

    public async Task<Result<string>> DeleteAsync(ApplicationUser user)
    {
        if((await userManager.DeleteAsync(user)).Succeeded)
        {
            return Result<string>.Success("User deleted");
        }
        else
        {
            return Result<string>.Failure([new Error("Delete-User", "User deletion failed")]);
        }
    }
}
