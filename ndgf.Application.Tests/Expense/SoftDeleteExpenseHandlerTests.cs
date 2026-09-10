using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Expense;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;
using NSubstitute;
namespace ndgf.Application.Tests.Expense;

public class SoftDeleteExpenseHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSucceed()
  {
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    ILogger<SoftDeleteExpenseHandler> logger = Substitute.For<ILogger<SoftDeleteExpenseHandler>>();

    var userId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var amount = 100m;
    var description = "description";
    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 100)
    };

    var expectedExpense = Domain.Entities.Expense.Create(userId, expendedPartInput, amount, description, groupId);
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    expenseRepository.GetExpenseByIdAsync(Arg.Any<Guid>()).Returns(expectedExpense);
    
    var handler = new SoftDeleteExpenseHandler(expenseRepository, groupMemberRepository, logger);
    var command = new SoftDeleteExpenseCommand(userId, groupId, expenseId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.True(result.IsSuccess);
    Assert.NotNull(expectedExpense.DeletedAt);
    await expenseRepository.Received(1).UpdateAsync(expectedExpense);
  }

  [Fact]
  public async Task HandleAsync_WithMemberNotInGroup_ShouldReturnFailureResult()
  {
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    ILogger<SoftDeleteExpenseHandler> logger = Substitute.For<ILogger<SoftDeleteExpenseHandler>>();
    
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    
    var handler = new SoftDeleteExpenseHandler(expenseRepository, groupMemberRepository, logger);
    var command = new SoftDeleteExpenseCommand(userId, groupId, expenseId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await expenseRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<Domain.Entities.Expense>());
  }
  
  [Fact]
  public async Task HandleAsync_WithExpenseNotFound_ShouldReturnFailureResult()
  {
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    ILogger<SoftDeleteExpenseHandler> logger = Substitute.For<ILogger<SoftDeleteExpenseHandler>>();
    
    var userId = Guid.NewGuid();
    var expenseId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    expenseRepository.GetExpenseByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.Expense?)null);
    
    var handler = new SoftDeleteExpenseHandler(expenseRepository, groupMemberRepository, logger);
    var command = new SoftDeleteExpenseCommand(userId, groupId, expenseId);
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await expenseRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<Domain.Entities.Expense>());
  }
}