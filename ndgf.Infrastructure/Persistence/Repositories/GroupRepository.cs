using Microsoft.EntityFrameworkCore;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class GroupRepository(NdgfDbContext context) : IGroupRepository
{
  public async Task<Group> AddAsync(Group group)
  {
    await context.Groups.AddAsync(group);
    await context.SaveChangesAsync();
    return group;
  }

  public async Task<Group?> GetGroupByIdAsync(Guid groupId)
  {
    return await context.Groups.FindAsync(groupId);
  }
}