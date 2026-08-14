using ndgf.Application.Commands.Expense;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Common;
using NSubstitute;

namespace ndgf.Application.Tests.Expense;

public class CreateExpenseHandlerTests
{
  [Fact]
  public async Task HandlerAsync_WithValidCredentials_ReturnSuccess()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(userId, 100)
    };

    var expectedExpense = Domain.Entities.Expense.Create(userId, expendedPartInput, amount, description, groupId);
    var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Jack", "Leonardo");

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(true, true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedUser);
    expenseRepository.AddAsync(Arg.Any<Domain.Entities.Expense>()).Returns(expectedExpense);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository);
    var command = new CreateExpenseCommand(userId, expendedPartInput, amount, description, groupId);

    var result = await handler.HandleAsync(command);

    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);

    var userInfo = Assert.Single(result.Value.UserExpenseInfoResults);

    Assert.Equal(expectedUser.Pseudo, userInfo.Pseudo);
    Assert.Equal(250m, userInfo.Amount);
    Assert.Equal(100, userInfo.Percentage);

    Assert.Equal(expectedUser.Pseudo, result.Value.PayerPseudo);
    Assert.Equal(expectedUser.Email, result.Value.PayerEmail);
  }
}