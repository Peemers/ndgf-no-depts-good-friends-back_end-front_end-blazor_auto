using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IRefundRepository
{
  Task<Refund> AddAsync(Refund refund);
  Task<IEnumerable<Refund>> GetAllGroupRefundAsync(Guid groupId);
}