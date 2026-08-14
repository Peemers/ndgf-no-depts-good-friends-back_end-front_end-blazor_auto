namespace ndgf.Application.Models.Expense;

public record CreateExpenseResult(Domain.Entities.Expense Expense, List<UserExpenseInfoResult> UserExpenseInfoResults, string PayerPseudo, string PayerEmail);