namespace ndgf.Web.Models.Expense;

public record GetGroupBalanceResponseModel
{
  public required List<UserBalanceResponseModel> Balances { get; init; }
  public required List<SuggestedRepaymentResponseModel> SuggestedRepayments { get; init; }
};