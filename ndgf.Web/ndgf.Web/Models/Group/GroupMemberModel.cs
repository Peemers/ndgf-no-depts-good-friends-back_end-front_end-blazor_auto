namespace ndgf.Web.Models.Group;

public record GroupMemberModel
{
  public required Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public required string Email { get; init; }
}