namespace ndgf.Web.Models.Expense;

public record CreateExpenseResponseModel
{
  public required Guid Id { get; init; }
  public required decimal Amount { get; init; }
  public required string Description { get; init; }
  public required string PayerPseudo { get; init; }
  public required string PayerEmail { get; init; }
  public required List<UserExpenseInfoResponseModel> UserExpenseInfos { get; init; }
};