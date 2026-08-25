namespace ndgf.Web.Models.GroupHistory;

public record GroupTransactionSummaryModel
{
  public required Guid Id { get; init; }
  public required TransactionTypeModel Transaction { get; init; }
  public decimal Amount { get; init; }
  public string? Description { get; init; }
  public DateTime CreatedAt { get; init; }
  public required string MainActorPseudo { get; init; }
}