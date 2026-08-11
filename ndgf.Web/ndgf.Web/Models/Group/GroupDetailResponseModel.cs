namespace ndgf.Web.Models.Group;

public record GroupDetailResponseModel
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
  public required List<GroupMemberModel> Members { get; init; }
}