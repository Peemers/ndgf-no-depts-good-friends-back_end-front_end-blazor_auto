namespace ndgf.Api.Dtos.Expense.Response;

public record GetGroupBalanceResponseDto
{
  public required List<UserBalanceResponseDto> Balances { get; init; }
};