namespace ndgf.Api.Dtos.Expense.Response;
  
public record GetGroupExpensesByMemberResponseDto
{
  public required List<MemberExpensesShareResponseDto> Expenses { get; init; }
};