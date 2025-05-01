using Domain.Entities;

namespace Domain.Interfaces;

public interface IRoleQueryRepository
{
    Task<IEnumerable<ApplicationRole>> Get();
}