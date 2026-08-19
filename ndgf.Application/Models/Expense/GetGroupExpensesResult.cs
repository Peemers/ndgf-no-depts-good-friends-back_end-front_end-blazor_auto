using ndgf.Domain.Common;

namespace ndgf.Application.Models.Expense;

public record GetGroupExpensesResult(PagedResult<ExpenseSummary> Expenses);