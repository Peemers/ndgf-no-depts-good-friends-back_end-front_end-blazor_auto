using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.GroupHistory;
using ndgf.Application.Queries.GroupHistory;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.GroupHistory;

public class GetGroupHistoryHandler(
  IUserRepository userRepository,
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  IRefundRepository refundRepository)
{
  public async Task<Result<GetGroupHistoryResult>> HandleAsync(GetGroupHistoryQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      return Result<GetGroupHistoryResult>.Failure("Vous devez etre membre du groupe pour consulter l'historique complet.");
    }

    var expenses = await expenseRepository.GetAllGroupExpensesAsync(query.GroupId);
    var refunds = await refundRepository.GetAllGroupRefundAsync(query.GroupId);
    
    var transactionsSummary = new List<GroupTransactionSummary>();
    
    foreach (var expense in expenses)
    {
      var payer = await userRepository.GetUserByIdAsync(expense.UserId);
      if (payer is not null)
      {
        transactionsSummary.Add(new GroupTransactionSummary(
          expense.Id,
          TransactionType.Expense,
          expense.Amount,
          expense.Description,
          expense.CreatedAt,
          payer.Pseudo,
          null));
      }
    }

    foreach (var refund in refunds)
    {
      var payer = await userRepository.GetUserByIdAsync(refund.PayerId);
      var receiver = await userRepository.GetUserByIdAsync(refund.ReceiverId);
      if (payer is not null && receiver is not null)
      {
        transactionsSummary.Add(new GroupTransactionSummary(
          refund.Id,
          TransactionType.Refund,
          refund.Amount,
          refund.Description,
          refund.CreatedAt,
          payer.Pseudo,
          receiver.Pseudo));
      }
    }
    
    var sortedTransactions = query.SortDescending
      ? transactionsSummary.OrderByDescending(t => t.CreatedAt).ToList()
      : transactionsSummary.OrderBy(t => t.CreatedAt).ToList();
    
    var pagedTransactions = sortedTransactions
      .Skip((query.PageNumber - 1) * query.PageSize)
      .Take(query.PageSize)
      .ToList();
    
    var summaryCount = transactionsSummary.Count;

    var totalPages = (int)Math.Ceiling(summaryCount / (double)query.PageSize);

    var pagedResult = new PagedResult<GroupTransactionSummary>(
      pagedTransactions,
      summaryCount,
      query.PageNumber,
      query.PageSize,
      totalPages);

    var result = new GetGroupHistoryResult(pagedResult);
    
    return Result<GetGroupHistoryResult>.Success(result);
  }
}