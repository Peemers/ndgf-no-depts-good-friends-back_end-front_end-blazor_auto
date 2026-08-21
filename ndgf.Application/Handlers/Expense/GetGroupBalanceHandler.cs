using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupBalanceHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository)
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
    
    var userBalance = new List<UserBalanceResult>();

    foreach (var member in groupMembers)
    {
      decimal balance = 0;
      var user = await userRepository.GetUserByIdAsync(member.UserId);

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
    
    var result = new GetGroupBalanceResult(userBalance);
    
    return Result<GetGroupBalanceResult>.Success(result);
  }
}