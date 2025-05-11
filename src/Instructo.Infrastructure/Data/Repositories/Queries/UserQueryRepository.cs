using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories.Queries;

public class UserQueryRepository(AppDbContext dbContext) : IUserQueries
{
    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        return await dbContext.Users.FindAsync(email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        return await dbContext.Users.ToListAsync();
    }

    public async Task<ApplicationUser> GetUserByIdAsync(Guid userId)
    {
        return await dbContext.Users.FindAsync(userId);
    }


    public async Task<bool> IsEmailUnique(string email)
    {
        return dbContext.Users.Any(x => x.Email == email);
    }
}