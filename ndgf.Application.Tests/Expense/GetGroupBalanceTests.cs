using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;
using NSubstitute;

namespace ndgf.Application.Tests.Expense;

public class GetGroupBalanceTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSuccess()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";
    var balance = -250m;

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(userId, 100)
    };

    var userBalance = new List<UserBalanceResult>
    {
      new(userId, "Toto", balance)
    };

    var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234=", "Toto", "Thierry", "Leman");
    var expectedGroupMember = GroupMember.Create(userId, groupId);
    var expectedExpense = Domain.Entities.Expense.Create(payerId, expendedPartInput, amount, description, groupId);


    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>()).Returns(new List<GroupMember> { expectedGroupMember });
    expenseRepository.GetAllActiveGroupExpensesAsync(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Expense> { expectedExpense });
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new GetGroupBalanceHandler(groupMemberRepository, expenseRepository, balanceCalculator);
    var query = new GetGroupBalanceQuery(groupId, userId);

    var result = await handler.HandleAsync(query);

    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);

    var balanceResult = Assert.Single(result.Value.Balances);
    Assert.Equal(expectedUser.Pseudo, balanceResult.Pseudo);
    Assert.Equal(-250m, balanceResult.Balance);
    Assert.Equal(250m, result.Value.TotalExpenses);
    Assert.Equal(1, result.Value.ExpenseCount);
  }

  [Fact]
  public async Task HandleAsync_WithUserNotInGroup_ShouldReturnFailure()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    

    var handler = new GetGroupBalanceHandler(groupMemberRepository, expenseRepository, balanceCalculator);
    var query = new GetGroupBalanceQuery(groupId, userId);
    var result = await handler.HandleAsync(query);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithRefund_ShouldAdjustBalance()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(userId, 100)
    };
    
    var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234=", "Toto", "Thierry", "Leman");
    var expectedGroupMember = GroupMember.Create(userId, groupId);
    var expectedExpense = Domain.Entities.Expense.Create(payerId, expendedPartInput, amount, description, groupId);
    
    var userBalance = new List<UserBalanceResult>
    {
      new(userId, expectedUser.Pseudo, -150m)
    };
    
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>()).Returns(new List<GroupMember> { expectedGroupMember });
    expenseRepository.GetAllActiveGroupExpensesAsync(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Expense> { expectedExpense });

    var handler = new GetGroupBalanceHandler(groupMemberRepository, expenseRepository, balanceCalculator);
    var query = new GetGroupBalanceQuery(groupId, userId);

    var result = await handler.HandleAsync(query);

    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);

    var balanceResult = Assert.Single(result.Value.Balances);
    Assert.Equal(-150m, balanceResult.Balance);
  }
}