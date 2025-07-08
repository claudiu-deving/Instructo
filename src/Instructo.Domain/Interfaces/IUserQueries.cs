using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserQueries
{
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<ApplicationUser>> GetUsers();
    Task<bool> IsEmailUnique(string email);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
}