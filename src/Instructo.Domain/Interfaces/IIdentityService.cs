using Instructo.Domain.Dtos.User;
using Instructo.Domain.Entities;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;

namespace Instructo.Domain.Interfaces;
public interface IIdentityService
{
    Task<Result<string>> AddToRoleAsync(ApplicationUser user, string role);
    Task<Result<string>> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    Task<Result<string>> CheckPasswordSignInAsync(ApplicationUser user, string password);
    Task<Result<string>> CreateAsync(ApplicationUser user, string password);
    Task<Result<string>> DeleteAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByIdAsync(Guid id);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<Result<string>> UpdateAsync(ApplicationUser user);
}