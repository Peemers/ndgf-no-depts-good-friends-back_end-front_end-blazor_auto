using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class RefundRepository(NdgfDbContext context) : IRefundRepository
{
  public async Task<Refund> AddAsync(Refund refund)
  {
    await context.Refunds.AddAsync(refund);
    await context.SaveChangesAsync();
    return refund;
  }
}