namespace ndgf.Api.Dtos.Group.Response;

public record CreateGroupResponseDto
{
  public Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
};