using ndgf.Api.Dtos.Expense.Response;
using ndgf.Application.Models.Expense;

namespace ndgf.Api.Mappers.Expense;

public static class GetGroupExpensesByMemberMapper
{
  public static GetGroupExpensesByMemberResponseDto ToResponseDto(this GetGroupExpensesByMemberResult result)
  {
    return new GetGroupExpensesByMemberResponseDto
    {
      Expenses = result.Expenses.Select(m => new MemberExpensesShareResponseDto()
      {
        Pseudo = m.Pseudo,
        Total = m.Total,
        UserId = m.UserId
      }).ToList()
    };
  }
}

//todo je viens de finir le mapper et les dto, endpoint et web la prochaine session.