using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Security;

public interface IJwtService
{
  string GenerateAccessToken(User user);
  string GenerateRefreshToken();
}