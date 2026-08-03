using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ndgf.Web.Services.Auth;

public class CustomAuthenticationStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var token = await GetTokenFromCookiesAsync();

    if (string.IsNullOrWhiteSpace(token))
    {
      return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
    
    var claims = ParseClaimsFromJwt(token);
    var identity = new ClaimsIdentity(claims, "jwt");
    var user = new ClaimsPrincipal(identity);

    return new AuthenticationState(user);
  }

  public void NotifyUserAuthentication()
  {
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
  }

  public void NotifyUserLogout()
  {
    var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
  }

  private async Task<string?> GetTokenFromCookiesAsync()
  {
    return await jsRuntime.InvokeAsync<string?>("ndgfAuth.getCookie", "accessToken");
  }

  private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
  {
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(jwt);

    return token.Claims;
  }
}