using Domain.Entities;

namespace Infrastructure.Data.Repositories.Queries;

public interface IRoleQueryRepository
{
    Task<IEnumerable<ApplicationRole>> Get();
}