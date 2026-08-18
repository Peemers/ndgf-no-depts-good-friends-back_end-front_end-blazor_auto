namespace ndgf.Web.Models.Expense;

public record ExpensePartInputModel
{
  public required Guid UserId { get; set; }
  public required int Percentage { get; set; }
};