using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ndgf.Web.Services.Auth;

//correction IA

public class CustomAuthenticationStateProvider(
  IJSRuntime jsRuntime,
  IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
  public override async Task<AuthenticationState> GetAuthenticationStateAsync()
  {
    var token = GetTokenFromHttpContext() ?? await GetTokenFromCookiesAsync();
    
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

  private string? GetTokenFromHttpContext()
  {
    return httpContextAccessor.HttpContext?.Request.Cookies["accessToken"];
  }

  private async Task<string?> GetTokenFromCookiesAsync()
  {
    try
    {
      return await jsRuntime.InvokeAsync<string?>("ndgfAuth.getCookie", "accessToken");
    }
    catch (InvalidOperationException)
    {
      return null;
    }
  }

  private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
  {
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(jwt);
    return token.Claims;
  }
}