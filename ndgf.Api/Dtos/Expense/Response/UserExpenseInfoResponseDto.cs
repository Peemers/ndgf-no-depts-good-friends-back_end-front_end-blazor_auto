namespace ndgf.Api.Dtos.Expense.Response;

public record UserExpenseInfoResponseDto
{
  public Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public int Percentage { get; init; }
  public decimal Amount { get; init; }
  public required string Email { get; init; }
};