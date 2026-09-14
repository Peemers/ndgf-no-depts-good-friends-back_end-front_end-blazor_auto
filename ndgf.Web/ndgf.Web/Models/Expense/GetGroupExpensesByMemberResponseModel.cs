namespace ndgf.Web.Models.Expense;

public record GetGroupExpensesByMemberResponseModel
{
  public required List<MemberExpenseShareResponseModel> Expenses { get; init; }
};