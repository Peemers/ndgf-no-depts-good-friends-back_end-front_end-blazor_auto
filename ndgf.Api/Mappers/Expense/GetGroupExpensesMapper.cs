using ndgf.Api.Dtos.Common;
using ndgf.Api.Dtos.Expense.Response;
using ndgf.Application.Models.Expense;

namespace ndgf.Api.Mappers.Expense;

public static class GetGroupExpensesMapper
{
  public static GetGroupExpensesResponseDto ToResponseDto(this GetGroupExpensesResult result)
  {
    var items = result.Expenses.Items.Select(e => new ExpenseSummaryDto
    {
      Id = e.Id,
      Amount = e.Amount,
      Description = e.Description,
      PayerPseudo = e.PayerPseudo,
      ParticipantCount = e.ParticipantCount,
      CreatedAt = e.CreatedAt
    }).ToList();

    var pagedDto = new PagedResultDto<ExpenseSummaryDto>
    {
      Items = items,
      TotalCount = result.Expenses.TotalCount,
      PageNumber = result.Expenses.PageNumber,
      PageSize = result.Expenses.PageSize,
      TotalPages = result.Expenses.TotalPages
    };

    return new GetGroupExpensesResponseDto { Expenses = pagedDto };
  }
}