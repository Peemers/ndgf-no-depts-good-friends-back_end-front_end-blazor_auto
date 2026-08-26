using Microsoft.EntityFrameworkCore;
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

  public async Task<IEnumerable<Refund>> GetAllGroupRefundAsync(Guid groupId)
  {
    return await context.Refunds
      .Where(r => r.GroupId == groupId)
      .ToListAsync();
  }

  public async Task<IEnumerable<Refund>> GetAllActiveGroupRefundAsync(Guid groupId)
  {
    return await context.Refunds
      .Where(r => r.GroupId == groupId && r.DeletedAt == null)
      .ToListAsync();
  }
}