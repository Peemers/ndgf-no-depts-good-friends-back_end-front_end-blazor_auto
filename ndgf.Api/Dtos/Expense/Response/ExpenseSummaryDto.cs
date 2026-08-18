namespace ndgf.Api.Dtos.Expense.Response;

public record ExpenseSummaryDto
{
  public required Guid Id { get; init; }
  public required decimal Amount { get; init; }
  public required string Description { get; init; }
  public required string PayerPseudo { get; init; }
  public required int ParticipantCount { get; init; }
  public required DateTime CreatedAt { get; init; }
}