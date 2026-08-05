using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ndgf.Application.Interfaces.Security;
using ndgf.Domain.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ndgf.Infrastructure.Security;

public class JwtService(IConfiguration configuration) : IJwtService
{
  public string GenerateAccessToken(User user)
  {
    var jwtSettings = configuration.GetSection("Jwt");
    var secret = jwtSettings.GetSection("Secret").Value;
    var issuer = jwtSettings.GetSection("Issuer").Value;
    var audience = jwtSettings.GetSection("Audience").Value;
    var expiryMinutes = int.Parse((string)jwtSettings["AccessTokenExpiryMinutes"]!);
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((string)secret!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email),
    };

    var token = new JwtSecurityToken(
      issuer: issuer,
      audience: audience,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
      signingCredentials: credentials
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public string GenerateRefreshToken()
  {
    var randomBytes = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomBytes);
    return Convert.ToBase64String(randomBytes);
  }
}