namespace ndgf.Application.Models.Group;

public record GetGroupDetailsResult(Domain.Entities.Group Group, List<GroupMemberInfoResult> Members);