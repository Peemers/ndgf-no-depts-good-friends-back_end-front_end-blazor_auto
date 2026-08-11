using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IGroupMemberRepository
{
  Task<GroupMember> AddAsync(GroupMember groupMember);
  Task<bool> IsMemberAsync(Guid userId, Guid groupId);
  Task<IEnumerable<GroupMember>> GetMemberByGroupIdAsync(Guid groupId);
  Task<IEnumerable<GroupMember>> GetMembershipsByUserIdAsync(Guid userId);
}