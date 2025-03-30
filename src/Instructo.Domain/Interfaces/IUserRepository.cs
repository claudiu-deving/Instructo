using Instructo.Domain.Entities;
using Instructo.Domain.Shared;

using MediatR;

namespace Instructo.Domain.Interfaces;
public interface IUserRepository
{
    Task<Result<string>> AddAsync(ApplicationUser user);
    Task<Result<Unit>> DeleteAsync(ApplicationUser user);
    Task<Result<IEnumerable<ApplicationUser>>> GetAllAsync();
    Task<Result<ApplicationUser>> GetByIdAsync(string id);
    Task<bool> IsEmailUnique(string email);
}