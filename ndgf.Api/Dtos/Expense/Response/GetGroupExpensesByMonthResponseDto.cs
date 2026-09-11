namespace ndgf.Api.Dtos.Expense.Response;

public record GetGroupExpensesByMonthResponseDto
{
  public required List<MonthlyExpensesResponseDto> MonthlyExpenses { get; init; }
};