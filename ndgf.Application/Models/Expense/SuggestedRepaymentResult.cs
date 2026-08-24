namespace ndgf.Application.Models.Expense;

public record SuggestedRepayment(Guid PayerId, string PayerPseudo, Guid ReceiverId, string ReceiverPseudo, decimal Amount);