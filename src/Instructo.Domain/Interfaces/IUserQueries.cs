using Instructo.Domain.Dtos;

namespace Instructo.Domain.Interfaces;

public interface IUserQueries
{
    Task<UserReadDto> GetUsersByIdAsync(string userId);
    Task<IEnumerable<UserReadDto>> GetUsers();
    Task<UserReadSuperDto> GetUsersByIdBySuperAsync(string userId);
    Task<IEnumerable<UserReadSuperDto>> GetUsersBySuper();
    Task<UserReadDto> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUnique(string email);
}