namespace ndgf.Api.Dtos.User.Request;

public record RefreshTokenRequestDto
{
  public required string RefreshToken { get; init; }
};