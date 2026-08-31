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
      [FromForm] string pseudo,
      [FromForm] string accessToken, 
      [FromForm] string refreshToken) =>
    {
      var claims = new List<Claim>
      {
        new(ClaimTypes.NameIdentifier, userId),
        new(ClaimTypes.Email, email),
        new("Pseudo", pseudo),
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
    
    app.MapPost("/auth/refresh-cookie", async (
        HttpContext httpContext,
        RefreshCookieRequest request) =>
      {
        var identity = (ClaimsIdentity)httpContext.User.Identity!;

        var oldAccessTokenClaim = identity.FindFirst("AccessToken");
        var oldRefreshTokenClaim = identity.FindFirst("RefreshToken");

        if (oldAccessTokenClaim is not null) identity.RemoveClaim(oldAccessTokenClaim);
        if (oldRefreshTokenClaim is not null) identity.RemoveClaim(oldRefreshTokenClaim);

        identity.AddClaim(new Claim("AccessToken", request.AccessToken));
        identity.AddClaim(new Claim("RefreshToken", request.RefreshToken));

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, httpContext.User);

        return Results.Ok();
      })
      .RequireAuthorization();

    return app;
  }
}