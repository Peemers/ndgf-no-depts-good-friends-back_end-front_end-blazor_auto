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

    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 100)
    };

    var expectedExpense = Domain.Entities.Expense.Create(payerId, expendedPartInput, amount, description, groupId);
    var expectedUser = Domain.Entities.User.Create("test@test.be", "Test1234!", "Toto", "Jack", "Leonardo");

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(true, true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedUser);
    expenseRepository.AddAsync(Arg.Any<Domain.Entities.Expense>()).Returns(expectedExpense);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expendedPartInput, amount, description, groupId);

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

  [Fact]
  public async Task HandleAsync_WithUserIsNotMember_ReturnsError()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";
    
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false, false);
    
    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 50)
    };
    
    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expendedPartInput, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }
  
  [Fact]
  public async Task HandleAsync_WithPayerNotFound_ReturnsError()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    
    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";
    
    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 50)
    };

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true, true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.User?)null);
    
    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expendedPartInput, amount, description, groupId);
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await expenseRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.Expense>());
  }
}