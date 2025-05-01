using Domain.Dtos.User;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserQueries
{
    Task<ApplicationUser> GetUsersByIdAsync(Guid userId);
    Task<IEnumerable<ApplicationUser>> GetUsers();
    Task<ApplicationUser?> GetUsersByIdBySuperAsync(Guid userId);
    Task<IEnumerable<ApplicationUser>> GetUsersBySuper();
    Task<ApplicationUser> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUnique(string email);
}