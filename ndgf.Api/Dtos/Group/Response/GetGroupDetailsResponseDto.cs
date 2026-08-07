namespace ndgf.Api.Dtos.Group.Response;

public record GetGroupDetailsResponseDto
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public string? Description { get; init; }
  public required List<GroupMemberInfoResultDto> Members { get; init; }
}

public record GroupMemberInfoResultDto
{
    public required Guid UserId { get; init; }
    public required string Pseudo { get; init; }
    public required string Email { get; init; }
}