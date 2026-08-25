namespace ndgf.Application.Queries.GroupHistory;

public record GetGroupHistoryQuery(Guid GroupId, Guid UserId, int PageNumber, int PageSize, bool SortDescending);