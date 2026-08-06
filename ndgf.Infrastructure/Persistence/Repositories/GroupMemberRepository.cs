using Microsoft.EntityFrameworkCore;
using ndgf.Application.Interfaces.Repositories;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class GroupMemberRepository(NdgfDbContext context) : IGroupMemberRepository
{
  public async Task<Domain.Entities.GroupMember> AddAsync(Domain.Entities.GroupMember groupMember)
  {
    await context.GroupMembers.AddAsync(groupMember);
    await context.SaveChangesAsync();
    return groupMember;
  }

  public async Task<bool> IsMemberAsync(Guid userId, Guid groupId)
  {
    return await context.GroupMembers
      .AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
  }
}