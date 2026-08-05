using Microsoft.EntityFrameworkCore;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(NdgfDbContext context) : IRefreshTokenRepository
{
  public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
  {
    return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
  }

  public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
  {
    await context.RefreshTokens.AddAsync(refreshToken);
    await context.SaveChangesAsync();
    return refreshToken;
  }

  public async Task UpdateAsync(RefreshToken refreshToken)
  {
    context.RefreshTokens.Update(refreshToken);
    await context.SaveChangesAsync();
  }
}