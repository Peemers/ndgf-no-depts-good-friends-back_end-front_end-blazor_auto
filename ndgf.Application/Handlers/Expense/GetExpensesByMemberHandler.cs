using Microsoft.Extensions.Logging;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetExpensesByMemberHandler(
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IUserRepository userRepository,
  ILogger<GetExpensesByMemberHandler> logger)
{
  public async Task<Result<GetGroupExpensesByMemberResult>> HandleAsync(GetGroupExpensesByMemberQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de chargement de la somme des dépenses par membre du groupe ({GroupId}) échouée - ({UserId}) ne fait pas partie de ce groupe",
        query.GroupId, query.UserId);
      return Result<GetGroupExpensesByMemberResult>.Failure("Vous devez être membre du groupe pour charger le total des dépenses par membre");
    }

    var expenses = await expenseRepository.GetAllActiveGroupExpensesAsync(query.GroupId);
    var totalsByUser = new Dictionary<Guid, decimal>();
    foreach (var expense in expenses)
    {
      var amountByUser = expense.CalculateAmountsByUser();

      foreach (var amount in amountByUser)
      {
        if (totalsByUser.ContainsKey(amount.Key))
        {
          totalsByUser[amount.Key] += amount.Value;
        }
        else
        {
          totalsByUser[amount.Key] = amount.Value;
        }
      }
    }

    var shares = new List<MemberExpensesShareResult>();

    foreach (var entry in totalsByUser)
    {
      var user = await userRepository.GetUserByIdAsync(entry.Key);
      if (user is not null)
      {
        shares.Add(new MemberExpensesShareResult(user.Id, user.Pseudo, entry.Value));
      }
    }

    var result = new GetGroupExpensesByMemberResult(shares);

    logger.LogInformation("[Succès] Chargement de la somme des dépenses par membre de ({GroupId}) par : ({UserId}) reussie", query.GroupId, query.UserId);

    return Result<GetGroupExpensesByMemberResult>.Success(result);
  }
}