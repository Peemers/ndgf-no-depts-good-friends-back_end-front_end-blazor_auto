namespace ndgf.Api.Dtos.GroupHistory.Response;

public record GroupTransactionSummaryDto
{
  public required Guid Id { get; init; }
  public TransactionTypeDto Transaction { get; init; }
  public decimal Amount { get; init; }
  public string? Description { get; init; }
  public DateTime CreatedAt { get; init; }
  public required string MainActorPseudo { get; init; }
  public required string? ReceiverPseudo { get; init; }
};