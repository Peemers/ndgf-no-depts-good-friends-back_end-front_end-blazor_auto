namespace ndgf.Web.Models.User;

public class LoginUserResponseModel
{
  public Guid Id { get; init; }
  public required string Email { get; init; }
  public required string AccessToken { get; init; }
  public required string RefreshToken { get; init; }
}