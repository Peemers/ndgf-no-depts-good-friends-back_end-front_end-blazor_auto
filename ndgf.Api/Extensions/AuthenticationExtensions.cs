using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ndgf.Api.Extensions;

public static class AuthenticationExtensions
{
  public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    var jwtSettings = configuration.GetSection("Jwt");
    var secret = jwtSettings["Secret"];
    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    
    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = issuer,
          ValidateAudience = true,
          ValidAudience = audience,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!))
        };
      });

    services.AddAuthorization();

    return services;
  }
}