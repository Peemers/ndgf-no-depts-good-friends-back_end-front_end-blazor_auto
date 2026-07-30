namespace ndgf.Api.Dtos.User.Response;

public record LoginUserResponseDto
{
  public  Guid Id { get; init; }
  public required string Email { get; init; }
  public required string AccessToken { get; init; }
  public required string RefreshToken { get; init; }
}