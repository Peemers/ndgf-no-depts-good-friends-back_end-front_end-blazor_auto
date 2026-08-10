namespace ndgf.Web.Models.Group;

public record GetUserGroupsResponseDto
{
  public required List<UserGroupSummaryDto> Groups { get; init; }
}