namespace ndgf.Api.Dtos.User.Response;

public record RegisterUserResponseDto
{
  public Guid Id { get; init; }
  public required string Email { get; init; }
}