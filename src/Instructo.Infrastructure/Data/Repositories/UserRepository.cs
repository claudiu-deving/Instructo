using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Instructo.Domain.Entities;
using Instructo.Domain.ValueObjects;
using Instructo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Instructo.Domain.Shared;
using MediatR;

namespace Instructo.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext=dbContext;
        _mapper=mapper;
    }

    public async Task<Result<ApplicationUser>> GetByIdAsync(string id)
    {
        var userEntity = await _dbContext.Users
            .FirstOrDefaultAsync(s => s.Id==id);
        if(userEntity==null)
            return new Error[] { new Error(id, "User not found") };

        return userEntity;
    }

    public async Task<Result<IEnumerable<ApplicationUser>>> GetAllAsync()
    {
        var schoolEntities = await _dbContext.Users.ToListAsync();
        return schoolEntities;
    }

    public async Task<Result<string>> AddAsync(ApplicationUser user)
    {
        var userEntity = _mapper.Map<ApplicationUser>(user);
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        // Update the ID in the domain model after saving
        user.Id=UserId.Create(userEntity.Id);

        return user.Id;
    }

    public async Task<Result<Unit>> DeleteAsync(ApplicationUser user)
    {
        var userEntity = await _dbContext.Users.FindAsync(user.Id);
        if(userEntity!=null)
        {
            _dbContext.Users.Remove(userEntity);
            await _dbContext.SaveChangesAsync();
            return Unit.Value;
        }
        else
        {
            return new Error[] { new Error("U-D", "User not found") };
        }
    }

    public async Task<bool> IsEmailUnique(string email)
    {
        return await _dbContext.Users.AnyAsync(u=>u.Email == email);
    }
}
