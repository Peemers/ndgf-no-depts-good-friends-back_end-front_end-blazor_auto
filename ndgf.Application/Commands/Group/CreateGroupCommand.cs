namespace ndgf.Application.Commands.Group;

public record CreateGroupCommand(string Name, string? Description, Guid UserId);