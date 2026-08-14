using ndgf.Domain.Entities;

namespace ndgf.Application.Interfaces.Repositories;

public interface IExpenseRepository
{
  Task<Expense> AddAsync(Expense expense);
}