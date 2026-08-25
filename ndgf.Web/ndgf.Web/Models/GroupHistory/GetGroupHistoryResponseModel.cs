using ndgf.Web.Models.Common;
using ndgf.Web.Models.Expense;

namespace ndgf.Web.Models.GroupHistory;

public record GetGroupHistoryResponseModel
{
  public required PagedResultModel<GroupTransactionSummaryModel> TransactionSummary { get; init; }
}