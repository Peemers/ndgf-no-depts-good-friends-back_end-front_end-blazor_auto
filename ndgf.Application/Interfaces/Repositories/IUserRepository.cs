using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IUserRepository
{
  Task<bool> EmailAlreadyExistsAsync(string email);
  Task<User> AddAsync(User user);
}