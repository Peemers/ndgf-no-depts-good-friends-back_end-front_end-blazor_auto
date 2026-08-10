namespace ndgf.Web.Models.Group;

public record CreateGroupResponseModel
{
  public Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
}