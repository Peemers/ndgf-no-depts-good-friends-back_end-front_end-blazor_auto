namespace ndgf.Web.Models.Expense;

public record GetGroupExpensesByMonthResponseModel
{
  public required List<MonthlyExpensesResponseModel> MonthlyExpenses { get; init; }
};