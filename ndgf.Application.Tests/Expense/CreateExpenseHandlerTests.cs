using Microsoft.Extensions.Logging;
using ndgf.Application.Commands.Expense;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
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
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateExpenseHandler> logger = Substitute.For<ILogger<CreateExpenseHandler>>();
    IGeoCodingService geoCodingService = Substitute.For<IGeoCodingService>();

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

    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(true, true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedUser);
    expenseRepository.AddAsync(Arg.Any<Domain.Entities.Expense>()).Returns(expectedExpense);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository, groupRepository, geoCodingService, logger);
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
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateExpenseHandler> logger = Substitute.For<ILogger<CreateExpenseHandler>>();
    IGeoCodingService geoCodingService = Substitute.For<IGeoCodingService>();


    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false, false);

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 50)
    };

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository, groupRepository, geoCodingService, logger);
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
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateExpenseHandler> logger = Substitute.For<ILogger<CreateExpenseHandler>>();
    IGeoCodingService geoCodingService = Substitute.For<IGeoCodingService>();


    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 50)
    };

    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true, true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.User?)null);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository, groupRepository, geoCodingService, logger);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expendedPartInput, amount, description, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await expenseRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.Expense>());
  }

  [Fact]
  public async Task HandleAsync_WithGroupArchived_ShouldReturnError()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    ILogger<CreateExpenseHandler> logger = Substitute.For<ILogger<CreateExpenseHandler>>();
    IGeoCodingService geoCodingService = Substitute.For<IGeoCodingService>();


    var groupId = Guid.NewGuid();
    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var amount = 250m;
    var description = "Test Expense";

    var expendedPartInput = new List<ExpensePartInput>
    {
      new(payerId, 50)
    };

    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(true);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository, groupRepository, geoCodingService, logger);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expendedPartInput, amount, description, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await expenseRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.Expense>());
  }

  [Fact]
public async Task HandleAsync_WithCoordinates_ShouldCallGeoCodingServiceAndSetLocation()
{
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IExpenseRepository expenseRepository = Substitute.For<IExpenseRepository>();
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    IGeoCodingService geoCodingService = Substitute.For<IGeoCodingService>();
    ILogger<CreateExpenseHandler> logger = Substitute.For<ILogger<CreateExpenseHandler>>();

    var requestingUserId = Guid.NewGuid();
    var payerId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var amount = 100m;
    var description = "Test description";
    var latitude = 48.8566m;
    var longitude = 2.3522m;
    var expectedLocation = "Paris, France";

    var expensePartInputs = new List<ExpensePartInput>
    {
        new(payerId, 100)
    };

    var expectedPayer = Domain.Entities.User.Create("test@test.be", "hash", "Toto", "Jack", "Leonardo");

    groupRepository.IsArchivedAsync(Arg.Any<Guid>()).Returns(false);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(expectedPayer);
    geoCodingService.GetGeoLocationAsync(latitude, longitude).Returns(expectedLocation);
    expenseRepository.AddAsync(Arg.Any<Domain.Entities.Expense>())
        .Returns(callInfo => callInfo.Arg<Domain.Entities.Expense>()!);

    var handler = new CreateExpenseHandler(userRepository, expenseRepository, groupMemberRepository, groupRepository, geoCodingService, logger);
    var command = new CreateExpenseCommand(requestingUserId, payerId, expensePartInputs, amount, description, groupId, latitude, longitude);

    var result = await handler.HandleAsync(command);

    Assert.True(result.IsSuccess);
    Assert.Equal(expectedLocation, result.Value!.Expense.Location);
    await geoCodingService.Received(1).GetGeoLocationAsync(latitude, longitude);
}
}