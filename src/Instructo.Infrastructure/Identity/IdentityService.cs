using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Instructo.Domain.Dtos;
using Instructo.Domain.Entities;
using Instructo.Domain.Interfaces;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;

namespace Instructo.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager=userManager;
        _signInManager=signInManager;
        _jwtSettings=jwtSettings;
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user==null)
            return Result<string>.Failure([new Error("Auth-Login", "Invalid credentials")]);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if(!result.Succeeded)
            return Result<string>.Failure([new Error("Auth-Login", result.IsLockedOut ? "Account locked" : "Invalid credentials")]);

        var token = await GenerateJwtTokenAsync(user);
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> RegisterAsync(RegisterUserDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if(existingUser!=null)
            return Result<string>.Failure([new Error("Auth-Login", "Email already in use")]);

        var newUser = new ApplicationUser
        {
            Email=registerDto.Email,
            UserName=registerDto.Email,
            FirstName=registerDto.FirstName,
            LastName=registerDto.LastName,
            Created=DateTime.UtcNow,
            IsActive=true
        };

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);
        if(!result.Succeeded)
            return Result<string>.Failure(result.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());

        // Add to default role
        await _userManager.AddToRoleAsync(newUser, "Student"); // Default role

        var token = await GenerateJwtTokenAsync(newUser);
        return Result<string>.Success(token);
    }

    public async Task<Result<string>> ExternalLoginAsync(ExternalLoginInfo info)
    {
        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if(signInResult.Succeeded)
        {
            // User already has an account
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            var token = await GenerateJwtTokenAsync(user);
            return Result<string>.Success(token);
        }

        // Create a new user with external provider
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var existingUser = await _userManager.FindByEmailAsync(email);

        if(existingUser!=null)
        {
            // Add external login to existing user
            await _userManager.AddLoginAsync(existingUser, info);
            var token = await GenerateJwtTokenAsync(existingUser);
            return Result<string>.Success(token);
        }

        // Create completely new user
        var newUser = new ApplicationUser
        {
            Email=email,
            UserName=email,
            FirstName=info.Principal.FindFirstValue(ClaimTypes.GivenName),
            LastName=info.Principal.FindFirstValue(ClaimTypes.Surname),
            Created=DateTime.UtcNow,
            IsActive=true
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if(!createResult.Succeeded)
            return Result<string>.Failure(createResult.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());

        await _userManager.AddLoginAsync(newUser, info);
        await _userManager.AddToRoleAsync(newUser, "Student"); // Default role

        var jwtToken = await GenerateJwtTokenAsync(newUser);
        return Result<string>.Success(jwtToken);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.UserName)
    };

        // Add role claims
        foreach(var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(_jwtSettings.Value.ExpiryDays);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Result<string>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(changePasswordDto.Email);
        if(user==null)
            return Result<string>.Failure([new Error("Auth-Login", "Invalid credentials")]);

        var result = await _signInManager.CheckPasswordSignInAsync(user, changePasswordDto.CurrentPassword, lockoutOnFailure: true);
        if(!result.Succeeded)
            return Result<string>.Failure([new Error("Auth-Login", result.IsLockedOut ? "Account locked" : "Invalid credentials")]);

        var changePasswordRequest = await _signInManager.UserManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
        if(changePasswordRequest.Succeeded)
        {
            return Result<string>.Success("Password changed successfully");
        }
        else
        {
            return Result<string>.Failure(changePasswordRequest.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
        }
    }

    public Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        //We skip for now the email verification

        return Task.FromResult(Result<string>.Success("Email sent"));
    }

    public async Task<Result<string>> UpdateUserAsync(string id, UserUpdateDto userUpdateDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user==null)
            return Result<string>.Failure([new Error("Auth-Login", "Invalid credentials")]);

        if(userUpdateDto.FirstName is not null)
        {
            user.FirstName=userUpdateDto.FirstName;
        }

        if(userUpdateDto.LastName is not null)
        {
            user.LastName=userUpdateDto.LastName;
        }

        if(userUpdateDto.PhoneNumber is not null)
        {
            user.PhoneNumber=userUpdateDto.PhoneNumber;
        }

        var updateResult = await _userManager.UpdateAsync(user);

        if(updateResult.Succeeded)
        {
            return Result<string>.Success(id);
        }
        else
        {
            return Result<string>.Failure(updateResult.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
        }
    }
}
