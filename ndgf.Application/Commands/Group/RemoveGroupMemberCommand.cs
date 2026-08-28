namespace ndgf.Application.Commands.Group;

public record RemoveGroupMemberCommand(Guid RequestingUserId, Guid MemberToRemoveId, Guid GroupId);