namespace ndgf.Api.Dtos.Expense.Response;

public record CreateExpenseResponseDto
{
  public required Guid Id { get; init; }
  public required decimal Amount { get; init; }
  public required string Description { get; init; }
  public required string PayerPseudo { get; init; }
  public required string PayerEmail { get; init; }
  public required List<UserExpenseInfoResponseDto> UserExpenseInfos { get; init; }
}