namespace ndgf.Application.Models.Expense;

public record ExpenseSummary(Guid Id, decimal Amount, string Description, string PayerPseudo, int ParticipantCount, DateTime CreatedAt);