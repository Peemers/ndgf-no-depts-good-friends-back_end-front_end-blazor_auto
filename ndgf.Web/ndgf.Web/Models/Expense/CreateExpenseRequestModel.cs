using System.ComponentModel.DataAnnotations;

namespace ndgf.Web.Models.Expense;

public record CreateExpenseRequestModel
{
  [Range(0.01, 2500, ErrorMessage = "Le montant doit être compris entre 0.01 et 2500.")] 
  public decimal Amount { get; set; }

  [Required(ErrorMessage = "La déscription de la dépense est requise.")]
  public required string Description { get; set; }
  
  [Required(ErrorMessage = "Un membre du groupe doit être désigné pour valider la dépense.")]
  public required Guid UserId { get; set; }
  
  public required List<ExpensePartInputModel> ExpenseParts { get; set; }
};