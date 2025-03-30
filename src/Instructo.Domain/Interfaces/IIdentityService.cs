using Instructo.Domain.Dtos;
using Instructo.Domain.Shared;

using Microsoft.AspNetCore.Identity;

namespace Instructo.Domain.Interfaces;
public interface IIdentityService
{
    Task<Result<string>> ChangePassword(ChangePasswordDto changePasswordDto);
    Task<Result<string>> ExternalLoginAsync(ExternalLoginInfo info);
    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<Result<string>> LoginAsync(string email, string password);
    Task<Result<string>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<string>> UpdateUserAsync(string id, UserUpdateDto userUpdateDto);
}