namespace ndgf.Api.Dtos.Common;

public record PagedResultDto<T>
{
  public required List<T> Items { get; init; }
  public required int TotalCount { get; init; }
  public required int PageNumber { get; init; }
  public required int PageSize { get; init; }
  public required int TotalPages { get; init; }
}