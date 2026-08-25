using ndgf.Api.Dtos.Common;
using ndgf.Api.Dtos.GroupHistory.Response;
using ndgf.Application.Models.GroupHistory;

namespace ndgf.Api.Mappers.GroupHistory;

public static class GroupHistoryMapper
{
  public static GetGroupHistoryResponseDto ToResponseDto(this GetGroupHistoryResult result)
  {
    var items = result.TransactionSummary.Items.Select(t => new GroupTransactionSummaryDto
    {
      Id = t.Id,
      Transaction = t.Type == TransactionType.Expense ? TransactionTypeDto.Expense : TransactionTypeDto.Refund,
      Amount = t.Amount,
      Description = t.Description,
      CreatedAt = t.CreatedAt,
      MainActorPseudo = t.MainActorPseudo
    }).ToList();

    var pagedDto = new PagedResultDto<GroupTransactionSummaryDto>
    {
      Items = items,
      TotalCount = result.TransactionSummary.TotalCount,
      PageNumber = result.TransactionSummary.PageNumber,
      PageSize = result.TransactionSummary.PageSize,
      TotalPages = result.TransactionSummary.TotalPages
    };

    return new GetGroupHistoryResponseDto { TransactionSummary = pagedDto };
  }
}