using Domain.Dtos.User;

namespace Domain.Interfaces;

public interface IUserQueries
{
    Task<UserReadDto> GetUsersByIdAsync(Guid userId);
    Task<IEnumerable<UserReadDto>> GetUsers();
    Task<UserReadSuperDto> GetUsersByIdBySuperAsync(Guid userId);
    Task<IEnumerable<UserReadSuperDto>> GetUsersBySuper();
    Task<UserReadDto> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUnique(string email);
}