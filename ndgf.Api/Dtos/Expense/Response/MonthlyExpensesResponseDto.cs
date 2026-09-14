namespace ndgf.Api.Dtos.Expense.Response;

public record MonthlyExpensesResponseDto
{
  public int Month { get; init; }
  public int Year { get; init; }
  public decimal Total { get; init; }
};