namespace ndgf.Application.Models.Group;

public record UserGroupSummaryResult(Guid Id, string Name, string? Description, int MemberCount);