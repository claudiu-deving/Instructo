using Domain.Entities;
using Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class UserQueryRepository(AppDbContext dbContext) : IUserQueries
{
    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await dbContext.Users.Include(x => x.School).FirstOrDefaultAsync(user => user.Email==email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        var result = await dbContext.Users.ToListAsync();
        return result;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(Guid userId)
    {
        return await dbContext.Users.Include(x => x.School).FirstOrDefaultAsync(user => userId==user.Id);
    }


    public async Task<bool> IsEmailUnique(string email)
    {
        return await dbContext.Users.AnyAsync(x => x.Email==email);
    }
}