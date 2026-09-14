namespace ndgf.Web.Models.Expense;

public record MemberExpenseShareResponseModel
{
  public Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public decimal Total { get; init; }
}