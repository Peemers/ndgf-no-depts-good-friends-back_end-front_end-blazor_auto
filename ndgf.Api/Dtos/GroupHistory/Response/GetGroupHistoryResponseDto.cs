using ndgf.Api.Dtos.Common;

namespace ndgf.Api.Dtos.GroupHistory.Response;

public record GetGroupHistoryResponseDto
{
  public required PagedResultDto<GroupTransactionSummaryDto> TransactionSummary { get; init; }
};