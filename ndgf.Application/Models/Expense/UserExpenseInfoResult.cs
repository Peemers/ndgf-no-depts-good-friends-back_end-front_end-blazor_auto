namespace ndgf.Application.Models.Expense;

public record UserExpenseInfoResult(Guid UserId, string Pseudo, int Percentage, decimal Amount, string Email);