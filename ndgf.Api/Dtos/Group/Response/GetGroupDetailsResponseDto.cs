namespace ndgf.Api.Dtos.Group.Response;

public record GetGroupDetailsResponseDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
  public required List<GroupMemberInfoResultDto> Members { get; init; }
}