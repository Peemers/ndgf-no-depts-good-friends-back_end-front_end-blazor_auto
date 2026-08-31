namespace ndgf.Web.Models.User;

public class AuthUserResponseModel
{
  public Guid Id { get; init; }
  public required string Email { get; init; }
  public required string Pseudo { get; init; }
  public required string AccessToken { get; init; }
  public required string RefreshToken { get; init; }
}