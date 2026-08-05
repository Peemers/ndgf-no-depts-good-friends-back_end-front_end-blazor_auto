using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
  Task<RefreshToken?> GetRefreshTokenAsync(string token);
  Task<RefreshToken> AddAsync(RefreshToken refreshToken);
  Task UpdateAsync(RefreshToken refreshToken);
}