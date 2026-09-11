namespace ndgf.Api.Dtos.Expense.Response;

public record MemberExpensesShareResponseDto
{
  public Guid UserId { get; init; }
  public required string Pseudo { get; init; }
  public decimal Total { get; init; }
};