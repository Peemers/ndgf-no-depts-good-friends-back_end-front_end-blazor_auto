using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class SoftDeleteExpenseHandler(
  IExpenseRepository expenseRepository,
  IGroupMemberRepository groupMemberRepository,
  ILogger<SoftDeleteExpenseHandler> logger)
{
  public async Task<Result<bool>> HandleAsync(SoftDeleteExpenseCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de suppression d'une dépense du groupe échouée - ({UserId}) ne fait pas partie du groupe : ({GroupId}) ", command.UserId,
        command.GroupId);
      return Result<bool>.Failure("Vous devez être membre du groupe pour effacer une dépense");
    }

    var expense = await expenseRepository.GetExpenseByIdAsync(command.ExpenseId);
    if (expense is null)
    {
      logger.LogInformation("[Echec] Tentative de suppression de la dépense ({ExpenseId}) dans le groupe ({GroupId}) - dépense inexistante", command.ExpenseId,
        command.GroupId);
      return Result<bool>.Failure("Dépense introuvable");
    }

    if (expense.DeletedAt is not null)
    {
      logger.LogInformation("[Echec] Tentative de suppression de la dépense ({ExpenseId}) dans le groupe ({GroupId}) - dépense deja supprimée", command.ExpenseId,
        command.GroupId);
      return Result<bool>.Failure("Cette dépense est déjà supprimée");
    }

    expense.SoftDelete();

    await expenseRepository.UpdateAsync(expense);

    logger.LogInformation("[Succès] Suppression de la dépense ({ExpenseId}) dans le groupe ({GroupId}) réussie par ({UserId})", command.ExpenseId, command.GroupId,
      command.UserId);

    return Result<bool>.Success(true);
  }
}