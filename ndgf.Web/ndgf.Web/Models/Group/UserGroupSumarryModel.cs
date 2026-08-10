namespace ndgf.Web.Models.Group;

public record UserGroupSummaryDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
  public int MemberCount { get; init; }
}