using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IExpenseRepository
{
  Task<Expense> AddAsync(Expense expense);
  Task<IEnumerable<Expense>> GetGroupExpensesAsync(Guid groupId, int pageNumber, int pageSize, bool sortDescending);
  Task<int> GetGroupExpensesCountAsync(Guid groupId);
  Task<IEnumerable<Expense>> GetAllGroupExpensesAsync(Guid groupId);
}