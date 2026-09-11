using Microsoft.Extensions.Logging;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;
using NSubstitute;

namespace ndgf.Application.Tests.Expense;

public class GetGroupExpenseByMonthHandlerTests
{
  [Fact]
  public async Task HandleAsync_withValidData_ShouldReturnSuccessResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    ILogger<GetGroupExpensesByMonthHandler> logger = Substitute.For<ILogger<GetGroupExpensesByMonthHandler>>();
    
    var payerId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var amount = 100m;
    var description = "Test description";
    var groupId = Guid.NewGuid();
    
    var expendedPartInput = new List<ExpensePartInput>
    {
      new(userId, 100)
    };
    
    var expectedExpense = Domain.Entities.Expense.Create(payerId, expendedPartInput, amount, description, groupId);
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    expenseRepository.GetAllActiveGroupExpensesAsync(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Expense> { expectedExpense });
    
    var handler = new GetGroupExpensesByMonthHandler(groupMemberRepository, expenseRepository, logger);
    var query = new GetGroupExpensesByMonthQuery(groupId, userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    
    var monthResult = Assert.Single(result.Value!.MonthlyExpenses);
    Assert.Equal(100m, monthResult.Total);
    Assert.Equal(DateTime.UtcNow.Month, monthResult.Month);
    Assert.Equal(DateTime.UtcNow.Year, monthResult.Year);
  }

  [Fact]
  public async Task HandleAsync_WithMemberNotInGroup_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    ILogger<GetGroupExpensesByMonthHandler> logger = Substitute.For<ILogger<GetGroupExpensesByMonthHandler>>();
    
    var payerId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var amount = 100m;
    var description = "Test description";
    var groupId = Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    
    var handler = new  GetGroupExpensesByMonthHandler(groupMemberRepository, expenseRepository, logger);
    var query = new GetGroupExpensesByMonthQuery(groupId, userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }
}