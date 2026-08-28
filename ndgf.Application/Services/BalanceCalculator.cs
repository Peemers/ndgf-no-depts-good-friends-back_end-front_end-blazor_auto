using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;

namespace ndgf.Application.Services;

public class BalanceCalculator(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IRefundRepository refundRepository) : IBalanceCalculator
{
  public async Task<List<UserBalanceResult>> CalculateBalanceAsync(Guid groupId)
  {
    var expenses = await expenseRepository.GetAllActiveGroupExpensesAsync(groupId);
    var groupMembers = await groupMemberRepository.GetMemberByGroupIdAsync(groupId);
    var refunds = await refundRepository.GetAllActiveGroupRefundAsync(groupId); 
    var userBalance = new List<UserBalanceResult>();
    
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
    return userBalance;
  }
}