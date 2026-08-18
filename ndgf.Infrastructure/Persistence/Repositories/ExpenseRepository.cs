using Microsoft.EntityFrameworkCore;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;

namespace ndgf.Infrastructure.Persistence.Repositories;

public class ExpenseRepository(NdgfDbContext context) : IExpenseRepository
{
  public async Task<Expense> AddAsync(Expense expense)
  {
    await context.Expenses.AddAsync(expense);
    await context.SaveChangesAsync();
    return expense;
  }

  public async Task<IEnumerable<Expense>> GetGroupExpensesAsync(Guid groupId, int pageNumber, int pageSize, bool sortDescending)
  {
    var query = context.Expenses.Where(e => e.GroupId == groupId);

    query = sortDescending
      ? query.OrderByDescending(e => e.CreatedAt)
      : query.OrderBy(e => e.CreatedAt);

    return await query
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync();
  }

  public async Task<int> GetGroupExpensesCountAsync(Guid groupId)
  {
    return await context.Expenses.CountAsync(e => e.GroupId == groupId);
  }
}