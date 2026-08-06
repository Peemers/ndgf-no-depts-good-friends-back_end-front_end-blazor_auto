using Microsoft.EntityFrameworkCore;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class UserRepository(NdgfDbContext context) : IUserRepository
{
  public async Task<bool> EmailAlreadyExistsAsync(string email)
  {
    return await context.Users.AnyAsync(u => u.Email == email);
  }

  public async Task<bool> PseudoAlreadyExistsAsync(string pseudo)
  {
    return await context.Users.AnyAsync(u => u.Pseudo == pseudo);
  }

  public async Task<User> AddAsync(User user)
  {
    await context.Users.AddAsync(user);
    await context.SaveChangesAsync();
    return user;
  }

  public async Task<User?> GetUserByEmailAsync(string email)
  {
    return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
  }
}