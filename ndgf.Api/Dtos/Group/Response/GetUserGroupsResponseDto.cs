namespace ndgf.Api.Dtos.Group.Response;

public record GetUserGroupsResponseDto
{
  public required List<UserGroupSummaryDto> Groups { get; init; }
}