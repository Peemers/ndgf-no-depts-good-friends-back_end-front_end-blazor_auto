using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Queries.Expense;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;
using NSubstitute;

namespace ndgf.Application.Tests.Expense;

public class GetGroupExpensesHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSuccess()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();

    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var payerId = Guid.NewGuid();

    var expensePartInput = new List<ExpensePartInput> { new(payerId, 100) };
    var expense = Domain.Entities.Expense.Create(payerId, expensePartInput, 50m, "Test Expense", groupId);
    var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Jack", "Leonardo");

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    expenseRepository.GetGroupExpensesAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
      .Returns(new List<Domain.Entities.Expense> { expense });
    expenseRepository.GetGroupExpensesCountAsync(Arg.Any<Guid>()).Returns(25);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedUser);

    var handler = new GetGroupExpenseHandler(userRepository, groupMemberRepository, expenseRepository);
    var query = new GetGroupExpenseQuery(groupId, userId, 1, 10, true);

    var result = await handler.HandleAsync(query);

    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);

    var summary = Assert.Single(result.Value.Expenses.Items);
    Assert.Equal(expense.Id, summary.Id);
    Assert.Equal(expectedUser.Pseudo, summary.PayerPseudo);
    Assert.Equal(1, summary.ParticipantCount);

    Assert.Equal(25, result.Value.Expenses.TotalCount);
    Assert.Equal(1, result.Value.Expenses.PageNumber);
    Assert.Equal(10, result.Value.Expenses.PageSize);
    Assert.Equal(3, result.Value.Expenses.TotalPages);
  }

  [Fact]
  public async Task HandleAsync_WithUserNotInGroup_ShouldReturnError()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);
    
    var handler = new GetGroupExpenseHandler(userRepository, groupMemberRepository, expenseRepository);
    var query = new GetGroupExpenseQuery(groupId, userId, 1, 10, true);
    
    var result = await handler.HandleAsync(query);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }
}