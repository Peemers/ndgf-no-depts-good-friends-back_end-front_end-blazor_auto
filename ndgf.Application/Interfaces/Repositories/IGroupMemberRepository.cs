using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IGroupMemberRepository
{
  Task<GroupMember> AddAsync(GroupMember groupMember);
}