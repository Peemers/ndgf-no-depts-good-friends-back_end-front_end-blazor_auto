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
}