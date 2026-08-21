namespace ndgf.Api.Dtos.Expense.Response;

public record UserBalanceResponseDto
{
  public Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public decimal Balance { get; init; }
  
};