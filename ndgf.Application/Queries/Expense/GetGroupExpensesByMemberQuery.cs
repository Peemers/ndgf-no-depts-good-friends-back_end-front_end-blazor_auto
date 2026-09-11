namespace ndgf.Application.Queries.Expense;

public record GetGroupExpensesByMemberQuery(Guid GroupId, Guid UserId);