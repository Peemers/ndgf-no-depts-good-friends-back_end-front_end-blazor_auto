namespace ndgf.Web.Models.Expense;

public record UserBalanceResponseModel
{
  public Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public decimal Balance { get; init; }
};