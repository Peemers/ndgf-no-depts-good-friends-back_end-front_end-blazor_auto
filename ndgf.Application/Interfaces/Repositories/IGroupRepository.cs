using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IGroupRepository
{
  Task<Group> AddAsync(Group group);
}