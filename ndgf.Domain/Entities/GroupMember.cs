using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Entities;

public class GroupMember
{
  public Guid UserId { get; private set; }
  public Guid GroupId { get; private set; }
  public DateTime JoinedAt { get; private set; }
  
  private GroupMember(){}

  private GroupMember(Guid userId, Guid groupId, DateTime joinedAt)
  {
    UserId = userId;
    GroupId = groupId;
    JoinedAt = joinedAt;
  }

  public static GroupMember Create(Guid userId, Guid groupId)
  {
    if (userId == Guid.Empty)
    {
      throw new DomainException("UserId requis");
    }

    if (groupId == Guid.Empty)
    {
      throw new DomainException("GroupId requis");
    }

    return new GroupMember(userId, groupId, DateTime.UtcNow);
  }
}