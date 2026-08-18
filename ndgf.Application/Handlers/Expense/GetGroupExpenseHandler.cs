using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Application.Models.Group;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupExpenseHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository)
{
  public async Task<Result<GetGroupExpensesResult>> HandleAsync(GetGroupExpenseQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      return Result<GetGroupExpensesResult>.Failure("Vous devez être membre du groupe pour en consulter les dépenses.");
    }

    var groupExpenses = await expenseRepository.GetGroupExpensesAsync(query.GroupId, query.PageNumber, query.PageSize, query.SortDescending);

    var expensesCount = await expenseRepository.GetGroupExpensesCountAsync(query.GroupId);

    var expenseSummary = new List<ExpenseSummary>();

    foreach (var groupExpense in groupExpenses)
    {
      var user = await userRepository.GetUserByIdAsync(groupExpense.UserId);

      if (user is not null)
      {
        var participantCount = groupExpense.ExpenseParts.Count;
        
        expenseSummary.Add(new ExpenseSummary(
          groupExpense.Id,
          groupExpense.Amount,
          groupExpense.Description,
          user.Pseudo,
          participantCount,
          groupExpense.CreatedAt));
      }
    }

    var totalPages = (int)Math.Ceiling(expensesCount / (double)query.PageSize);

    var pagedResult = new PagedResult<ExpenseSummary>(
      expenseSummary,
      expensesCount,
      query.PageNumber,
      query.PageSize,
      totalPages);

    var result = new GetGroupExpensesResult(pagedResult);
    
    return Result<GetGroupExpensesResult>.Success(result);
  }
}