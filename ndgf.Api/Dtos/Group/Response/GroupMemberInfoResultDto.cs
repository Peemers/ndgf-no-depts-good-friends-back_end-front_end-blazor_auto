namespace ndgf.Api.Dtos.Group.Response;

public record GroupMemberInfoResultDto
{
  public required Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public required string Email { get; init; }
}