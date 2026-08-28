using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Application.Services;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupBalanceHandler(
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IBalanceCalculator balanceCalculator)
{
  public async Task<Result<GetGroupBalanceResult>> HandleAsync(GetGroupBalanceQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      return Result<GetGroupBalanceResult>.Failure("Vous devez être membre du groupe pour consulter toutes les dépenses.");
    }
    
    var expenses = await expenseRepository.GetAllActiveGroupExpensesAsync(query.GroupId);
    
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
    
    return Result<GetGroupBalanceResult>.Success(result);
  }
}