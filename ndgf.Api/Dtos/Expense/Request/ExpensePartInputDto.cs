namespace ndgf.Api.Dtos.Expense.Request;

public record ExpensePartInputDto
{
  public required Guid UserId { get; init; }
  public required int Percentage { get; init; }
};