using ndgf.Application.Commands.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class SoftDeleteExpenseHandler(
  IExpenseRepository expenseRepository,
  IGroupMemberRepository groupMemberRepository)
{
  public async Task<Result<bool>> HandleAsync(SoftDeleteExpenseCommand command)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(command.UserId, command.GroupId);
    if (!isMember)
    {
      return Result<bool>.Failure("Vous devez être membre du groupe pour effacer une dépense");
    }

    var expense = await expenseRepository.GetExpenseByIdAsync(command.ExpenseId);
    if (expense is null)
    {
      return Result<bool>.Failure("Dépense introuvable");
    }

    if (expense.DeletedAt is not null)
    {
      return Result<bool>.Failure("Cette dépense est déjà supprimée");
    }
    
    expense.SoftDelete();
    
    await expenseRepository.UpdateAsync(expense);
    
    return Result<bool>.Success(true);
  }
}