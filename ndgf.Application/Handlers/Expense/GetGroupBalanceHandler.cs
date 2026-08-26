using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupBalanceHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IRefundRepository refundRepository)
{
  public async Task<Result<GetGroupBalanceResult>> HandleAsync(GetGroupBalanceQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      return Result<GetGroupBalanceResult>.Failure("Vous devez être membre du groupe pour consulter toutes les dépenses.");
    }

    var groupMembers = await groupMemberRepository.GetMemberByGroupIdAsync(query.GroupId);

    var expenses = await expenseRepository.GetAllGroupExpensesAsync(query.GroupId);
    
    var refunds = await refundRepository.GetAllGroupRefundAsync(query.GroupId);
    
    var userBalance = new List<UserBalanceResult>();
    var totalExpenses = expenses.Sum(expense => expense.Amount);

    foreach (var member in groupMembers)
    {
      decimal balance = 0;
      var user = await userRepository.GetUserByIdAsync(member.UserId);

      foreach (var refund in refunds)
      {
        if (refund.PayerId == member.UserId)
        {
          balance += refund.Amount;
        }

        if (refund.ReceiverId == member.UserId)
        {
          balance -= refund.Amount;
        }
      }

      foreach (var expense in expenses)
      {
        if (expense.UserId == member.UserId)
        {
          balance += expense.Amount;
        }
        
        var part = expense.ExpenseParts.FirstOrDefault(ep => ep.UserId == member.UserId);
        if (part is not null)
        {
          balance -= expense.Amount * part.Percentage / 100m;
        }
      }
      if (user is not null)
      {
        userBalance.Add(new UserBalanceResult(user.Id, user.Pseudo, balance));
      }
    }
    
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
    
    var result = new GetGroupBalanceResult(userBalance, suggestedRepayments, totalExpenses);
    
    return Result<GetGroupBalanceResult>.Success(result);
  }
}