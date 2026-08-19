using ndgf.Web.Models.Common;

namespace ndgf.Web.Models.Expense;

public record GetGroupExpensesResponseModel
{
  public required PagedResultModel<ExpenseSummaryModel> Expenses { get; init; }
}