namespace ndgf.Application.Models.GroupHistory;

public record GroupTransactionSummary(
  Guid Id,
  TransactionType Type,
  decimal Amount,
  string? Description,
  DateTime CreatedAt,
  string MainActorPseudo);