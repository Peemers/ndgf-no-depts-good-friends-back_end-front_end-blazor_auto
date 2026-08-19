using ndgf.Api.Dtos.Common;

namespace ndgf.Api.Dtos.Expense.Response;

public record GetGroupExpensesResponseDto
{
  public required PagedResultDto<ExpenseSummaryDto> Expenses { get; init; }
}