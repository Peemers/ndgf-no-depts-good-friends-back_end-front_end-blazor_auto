namespace ndgf.Web.Models.User;

public class LoginUserResponseModel
{
  public Guid Id { get; set; }
  public required string Email { get; set; }
  public required string AccessToken { get; set; }
  public required string RefreshToken { get; set; }
}