using ndgf.Web.Models.User;

namespace ndgf.Web.Services.User;

public class UserApiClient(HttpClient httpClient)
{
  public async Task<HttpResponseMessage> RegisterAsync(RegisterUserModel model)
  {
    return await httpClient.PostAsJsonAsync("/api/users/register", model);
  }

  public async Task<HttpResponseMessage> LoginAsync(LoginUserModel model)
  {
    return await httpClient.PostAsJsonAsync("/api/users/login", model);
  }
}