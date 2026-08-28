using ndgf.Application.Models.Expense;

namespace ndgf.Application.Interfaces.Services;

public interface IBalanceCalculator
{
  Task<List<UserBalanceResult>> CalculateBalanceAsync(Guid groupId);
}