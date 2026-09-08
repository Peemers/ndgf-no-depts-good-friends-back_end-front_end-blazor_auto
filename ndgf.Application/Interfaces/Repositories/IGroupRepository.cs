using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IGroupRepository
{
  Task<Group> AddAsync(Group group);
  Task<Group?> GetGroupByIdAsync(Guid groupId);
  Task<bool> IsArchivedAsync(Guid groupId);
  Task UpdateAsync(Group group);
}