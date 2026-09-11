namespace ndgf.Application.Queries.Expense;

public record GetGroupExpensesByMonthQuery(Guid GroupId, Guid UserId);