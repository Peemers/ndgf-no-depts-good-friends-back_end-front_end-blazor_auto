using System.ComponentModel.DataAnnotations;

namespace ndgf.Api.Dtos.Expense.Request;

public record CreateExpenseRequestDto
{
  [Range(0.01, double.MaxValue)] 
  public decimal Amount { get; init; }

  [Required(ErrorMessage = "La déscription de la dépense est requise")]
  public required string Description { get; init; }

  public required Guid UserId { get; init; }
  
  public required List<ExpensePartInputDto> ExpenseParts { get; init; }
}