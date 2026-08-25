namespace ndgf.Api.Dtos.Expense.Response;

public record SuggestedRepaymentResponseDto
{
  public required Guid PayerId { get; init; }
  public required string PayerPseudo { get; init; }
  public required Guid ReceiverId { get; init; }
  public required string ReceiverPseudo { get; init; }
  public decimal Amount { get; init; }
};