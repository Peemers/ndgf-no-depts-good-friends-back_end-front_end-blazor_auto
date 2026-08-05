using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ndgf.Web.Endpoints;

public static class AuthEndpoints
{
  public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/auth/signin", async (
      HttpContext context, 
      [FromForm] string userId, 
      [FromForm] string email, 
      [FromForm] string accessToken, 
      [FromForm] string refreshToken) =>
    {
      var claims = new List<Claim>
      {
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Email, email),
        new("AccessToken", accessToken),
        new("RefreshToken", refreshToken)
      };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

      return Results.Redirect("/");
    });

    app.MapGet("/auth/signout", async (HttpContext context) =>
    {
      await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      return Results.Redirect("/login");
    });

    return app;
  }
}