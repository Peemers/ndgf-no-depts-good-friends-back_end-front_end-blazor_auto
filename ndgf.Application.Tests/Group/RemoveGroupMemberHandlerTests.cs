using ndgf.Application.Commands.Group;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Models.Expense;
using ndgf.Domain.Entities;
using NSubstitute;

namespace ndgf.Application.Tests.Group;

public class RemoveGroupMemberHandlerTests
{
  [Fact]
  public async Task HandleAsync_withValidData_ShouldReturnSuccessResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var memberToRemoveId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var balance = 0;

    var userBalance = new List<UserBalanceResult>()
    {
      new(memberToRemoveId, "Toto", balance)
    };

    var expectedRequestingGroupMember = GroupMember.Create(requestingUserId, groupId);
    var expectedGroupMemberToDelete = GroupMember.Create(memberToRemoveId, groupId);

    groupMemberRepository.IsMemberAsync(requestingUserId, groupId).Returns(true);
    groupMemberRepository.IsMemberAsync(memberToRemoveId, groupId).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>())
      .Returns(new List<GroupMember> { expectedRequestingGroupMember, expectedGroupMemberToDelete });
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, memberToRemoveId, groupId);


    var result = await handler.HandleAsync(command);

    Assert.True(result.IsSuccess);
    await groupMemberRepository.Received(1).RemoveAsync(expectedGroupMemberToDelete);
  }

  [Fact]
  public async Task HandleAsync_WithRequestingMemberNotInGroup_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var memberToRemoveId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, memberToRemoveId, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithMemberToRemoveNotInGroup_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var memberToRemoveId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(requestingUserId, Arg.Any<Guid>()).Returns(true);
    groupMemberRepository.IsMemberAsync(memberToRemoveId, Arg.Any<Guid>()).Returns(false);

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, memberToRemoveId, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_RemoveCreatorByAnotherMember_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var creatorId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var creatorMember = GroupMember.Create(creatorId, groupId);

    // attend un peu pour garantir un JoinedAt plus récent pour requestingUser
    await Task.Delay(10);

    var requestingMember = GroupMember.Create(requestingUserId, groupId);

    groupMemberRepository.IsMemberAsync(requestingUserId, groupId).Returns(true);
    groupMemberRepository.IsMemberAsync(creatorId, groupId).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>())
      .Returns(new List<GroupMember> { creatorMember, requestingMember });

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, creatorId, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithNonZeroBalance_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var memberToRemoveId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var requestingMember = GroupMember.Create(requestingUserId, groupId);
    var memberToRemove = GroupMember.Create(memberToRemoveId, groupId);

    var userBalance = new List<UserBalanceResult>
    {
      new(memberToRemoveId, "Toto", 50m)
    };

    groupMemberRepository.IsMemberAsync(requestingUserId, groupId).Returns(true);
    groupMemberRepository.IsMemberAsync(memberToRemoveId, groupId).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>())
      .Returns(new List<GroupMember> { requestingMember, memberToRemove });
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, memberToRemoveId, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }

  [Fact]
  public async Task HandleAsync_WithLastMember_ShouldReturnFailureResult()
  {
    IGroupMemberRepository groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    IBalanceCalculator balanceCalculator = Substitute.For<IBalanceCalculator>();

    var requestingUserId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var onlyMember = GroupMember.Create(requestingUserId, groupId);

    var userBalance = new List<UserBalanceResult>
    {
      new(requestingUserId, "Toto", 0m)
    };

    groupMemberRepository.IsMemberAsync(requestingUserId, groupId).Returns(true);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>())
      .Returns(new List<GroupMember> { onlyMember });
    balanceCalculator.CalculateBalanceAsync(Arg.Any<Guid>()).Returns(userBalance);

    var handler = new RemoveGroupMemberHandler(groupMemberRepository, balanceCalculator);
    var command = new RemoveGroupMemberCommand(requestingUserId, requestingUserId, groupId);

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
  }
}