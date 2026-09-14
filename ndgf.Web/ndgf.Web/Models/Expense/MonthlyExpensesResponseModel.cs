namespace ndgf.Web.Models.Expense;

public record MonthlyExpensesResponseModel
{
  public int Month { get; init; }
  public int Year { get; init; }
  public decimal Total { get; init; }
};