namespace ndgf.Application.Commands.Expense;

public record SoftDeleteExpenseCommand(Guid UserId, Guid GroupId, Guid ExpenseId);