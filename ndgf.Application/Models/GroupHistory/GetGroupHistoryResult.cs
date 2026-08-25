using ndgf.Domain.Common;

namespace ndgf.Application.Models.GroupHistory;

public record GetGroupHistoryResult(PagedResult<GroupTransactionSummary> TransactionSummary);