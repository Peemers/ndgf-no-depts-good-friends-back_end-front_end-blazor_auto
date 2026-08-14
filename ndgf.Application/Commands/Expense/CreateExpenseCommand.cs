using ndgf.Domain.Common;

namespace ndgf.Application.Commands.Expense;

public record CreateExpenseCommand(Guid UserId, List<ExpensePartInput> ExpensePartInputs, decimal Amount, string Description, Guid GroupId);