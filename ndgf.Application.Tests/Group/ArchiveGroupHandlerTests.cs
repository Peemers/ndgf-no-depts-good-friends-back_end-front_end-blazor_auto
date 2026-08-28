using ndgf.Application.Commands.Group;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;
using NSubstitute;

namespace ndgf.Application.Tests.Group;

public class ArchiveGroupHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidData_ShouldReturnSuccessResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var balance = 0;
    var expectedGroup = Domain.Entities.Group.Create("Tignes 2026", "déscription");
    var userBalance = new List<UserBalanceResult>
    {
      new(userId, "toto", balance)
    };

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns(expectedGroup);
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new ArchiveGroupHandler(groupMemberRepository, groupRepository, balanceCalculator);
    var command = new ArchiveGroupCommand(userId, groupId);
    var result = await handler.HandleAsync(command);

    Assert.True(result.IsSuccess);
    Assert.NotNull(expectedGroup.ArchivedAt);
  }

  [Fact]
  public async Task HandleAsync_WithUserNotInGroup_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

    var handler = new ArchiveGroupHandler(groupMemberRepository, groupRepository, balanceCalculator);
    var command = new ArchiveGroupCommand(userId, groupId);
    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithGroupNotFound_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.Group?)null);

    var handler = new ArchiveGroupHandler(groupMemberRepository, groupRepository, balanceCalculator);
    var command = new ArchiveGroupCommand(userId, groupId);
    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithNonZeroBalance_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var expectedGroup = Domain.Entities.Group.Create("Tignes 2026", "déscription");
    var userBalance = new List<UserBalanceResult>
    {
      new(userId, "toto", 50m)
    };

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns(expectedGroup);
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new ArchiveGroupHandler(groupMemberRepository, groupRepository, balanceCalculator);
    var command = new ArchiveGroupCommand(userId, groupId);
    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    Assert.Null(expectedGroup.ArchivedAt);
  }
}