namespace ndgf.Application.Commands.Group;

public record AddGroupMemberCommand(Guid UserId, Guid GroupId, string SearchValue);