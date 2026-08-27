namespace ndgf.Application.Models.Expense;

public record GetGroupBalanceResult(List<UserBalanceResult> Balances, List<SuggestedRepayment> SuggestedRepayments, decimal TotalExpenses, int ExpenseCount);