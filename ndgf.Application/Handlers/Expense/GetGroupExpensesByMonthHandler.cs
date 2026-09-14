using Microsoft.Extensions.Logging;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;

namespace ndgf.Application.Handlers.Expense;

public class GetGroupExpensesByMonthHandler(
  IGroupMemberRepository groupMemberRepository,
  IExpenseRepository expenseRepository,
  ILogger<GetGroupExpensesByMonthHandler> logger)
{
  public async Task<Result<GetGroupExpensesByMonthResult>> HandleAsync(GetGroupExpensesByMonthQuery query)
  {
    bool isMember = await groupMemberRepository.IsMemberAsync(query.UserId, query.GroupId);
    if (!isMember)
    {
      logger.LogInformation("[Echec] Tentative de chargement des dépenses mensuelles du groupe ({GroupId}) échouée - ({UserId}) ne fait pas partie de ce groupe",
        query.GroupId, query.UserId);
      return Result<GetGroupExpensesByMonthResult>.Failure("Vous devez être membre du groupe pour charger les dépenses mensuelles.");
    }

    var expenses = await expenseRepository.GetAllActiveGroupExpensesAsync(query.GroupId);

    var expensesByMonth = expenses
      .Where(e => e.CreatedAt >= DateTime.UtcNow.AddMonths(-12))
      .GroupBy(e => new { e.CreatedAt.Year, e.CreatedAt.Month })
      .Select(g => new
      {
        Year = g.Key.Year,
        Month = g.Key.Month,
        Total = g.Sum(e => e.Amount)
      })
      .OrderBy(g => g.Year)
      .ThenBy(g => g.Month);

    var monthlyResults = expensesByMonth
      .Select(m => new MonthlyExpensesResult(m.Year, m.Month, m.Total))
      .ToList();

    var result = new GetGroupExpensesByMonthResult(monthlyResults);

    logger.LogInformation("[Succès] Chargement des dépenses mensuelles de ({GroupId}) par : ({UserId}) reussie", query.GroupId, query.UserId);

    return Result<GetGroupExpensesByMonthResult>.Success(result);
  }
}