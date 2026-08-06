using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IUserRepository
{
  Task<bool> EmailAlreadyExistsAsync(string email);
  Task<bool> PseudoAlreadyExistsAsync(string pseudo);
  Task<User> AddAsync(User user);
  Task<User?> GetUserByEmailAsync(string email);
}