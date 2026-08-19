namespace ndgf.Application.Queries.Expense;

public record GetGroupExpenseQuery(Guid GroupId, Guid UserId, int PageNumber, int PageSize, bool SortDescending);