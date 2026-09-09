using Microsoft.Extensions.Logging;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupBalanceHandler(
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IBalanceCalculator balanceCalculator,
  ILogger<GetGroupBalanceHandler> logger)
{
  public async Task<Result<GetGroupBalanceResult>> HandleAsync(GetGroupBalanceQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      logger.LogInformation("Tentative de chargement de la balance du groupe échouée - ({UserId}) ne fait pas partie du groupe : ({GroupId}) ", query.UserId,
        query.GroupId);
      return Result<GetGroupBalanceResult>.Failure("Vous devez être membre du groupe pour consulter toutes les dépenses.");
    }

    var expenses = (await expenseRepository.GetAllActiveGroupExpensesAsync(query.GroupId)).ToList();

    var userBalance = await balanceCalculator.CalculateBalanceAsync(query.GroupId);

    var totalExpenses = expenses.Sum(expense => expense.Amount);

    var suggestedRepayments = new List<SuggestedRepayment>();

    var creditors = userBalance.Where(ub => ub.Balance > 0).ToList();
    var debtors = userBalance.Where(ub => ub.Balance < 0).ToList();

    int debtorIndex = 0;
    int creditorIndex = 0;

    while (debtorIndex < debtors.Count && creditorIndex < creditors.Count)
    {
      var debtorAmount = -debtors[debtorIndex].Balance;
      var creditorAmount = creditors[creditorIndex].Balance;

      var transferAmount = Math.Min(debtorAmount, creditorAmount);

      suggestedRepayments.Add(new SuggestedRepayment(
        debtors[debtorIndex].UserId, debtors[debtorIndex].Pseudo,
        creditors[creditorIndex].UserId, creditors[creditorIndex].Pseudo,
        transferAmount));

      debtors[debtorIndex] = debtors[debtorIndex] with { Balance = debtors[debtorIndex].Balance + transferAmount };
      creditors[creditorIndex] = creditors[creditorIndex] with { Balance = creditors[creditorIndex].Balance - transferAmount };

      if (debtors[debtorIndex].Balance == 0) debtorIndex++;
      if (creditors[creditorIndex].Balance == 0) creditorIndex++;
    }

    var expenseCount = expenses.Count();
    var result = new GetGroupBalanceResult(userBalance, suggestedRepayments, totalExpenses, expenseCount);
    
    logger.LogInformation("Chargement de la balance du groupe ({GroupId}) par : ({UserId}) reussie", query.UserId , query.GroupId);

    return Result<GetGroupBalanceResult>.Success(result);
  }
}