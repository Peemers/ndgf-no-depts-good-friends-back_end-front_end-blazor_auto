using ndgf.Api.Dtos.Expense.Response;
using ndgf.Application.Models.Expense;

namespace ndgf.Api.Mappers.Expense;

public static class GetGroupExpensesByMonthMapper
{
  public static GetGroupExpensesByMonthResponseDto ToResponseDto(this GetGroupExpensesByMonthResult result)
  {
    return new GetGroupExpensesByMonthResponseDto
    {
      MonthlyExpenses = result.MonthlyExpenses.Select(m => new MonthlyExpensesResponseDto
      {
        Month = m.Month,
        Year = m.Year,
        Total = m.Total
      }).ToList()
    };
  }
}