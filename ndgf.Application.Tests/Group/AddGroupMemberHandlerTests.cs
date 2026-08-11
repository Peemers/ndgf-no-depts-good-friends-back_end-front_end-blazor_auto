using ndgf.Application.Commands.Group;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Domain.Entities;
using NSubstitute;

namespace ndgf.Application.Tests.Group;

public class AddGroupMemberHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccessResult()
  {
    //Arrange

    var userRepository = Substitute.For<IUserRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var inviterId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var existingUser = Domain.Entities.User.Create("test@test.be", "passwordHash", "toto", "robert", "Berto");
    var expectedGroupMember = GroupMember.Create(existingUser.Id, groupId);

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(true, false);

    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(existingUser);

    groupMemberRepository.AddAsync(Arg.Any<GroupMember>()).Returns(expectedGroupMember);

    var handler = new AddGroupMemberHandler(userRepository, groupMemberRepository);
    var command = new AddGroupMemberCommand(inviterId, groupId, "nouveau@test.be");

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(existingUser.Id, result.Value.UserId);
    Assert.Equal(groupId, result.Value.GroupId);
  }

  [Fact]
  public async Task HandleAsync_WithUserNotFound_ShouldReturnFailureResult()
  {
    //Arrange

    var userRepository = Substitute.For<IUserRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var inviterId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(inviterId, groupId).Returns(true);
    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns((Domain.Entities.User?)null);

    var handler = new AddGroupMemberHandler(userRepository, groupMemberRepository);
    var command = new AddGroupMemberCommand(inviterId, groupId, "inconnu@test.be");

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await groupMemberRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.GroupMember>());
  }

  [Fact]
  public async Task HandleAsync_WithInviterNotInGroup_ShouldReturnFailureResult()
  {
    var userRepository = Substitute.For<IUserRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var inviterId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(inviterId, groupId).Returns(false);

    var handler = new AddGroupMemberHandler(userRepository, groupMemberRepository);
    var command = new AddGroupMemberCommand(inviterId, groupId, "inconnu@test.be");

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await groupMemberRepository.DidNotReceive().AddAsync(Arg.Any<GroupMember>());
    await userRepository.DidNotReceive().GetUserByEmailAsync(Arg.Any<string>());
    await userRepository.DidNotReceive().GetUserByPseudoAsync(Arg.Any<string>());
  }

  [Fact]
  public async Task HandleAsync_WithMemberAlreadyInGroup_ShouldReturnFailureResult()
  {
    var userRepository = Substitute.For<IUserRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    var inviterId = Guid.NewGuid();
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
      .Returns(true, true);

    var existingUser = Domain.Entities.User.Create("test@test.be", "passwordHash", "toto", "robert", "Berto");
    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(existingUser);


    var handler = new AddGroupMemberHandler(userRepository, groupMemberRepository);
    var command = new AddGroupMemberCommand(userId, groupId, "test@test.be");

    var result = await handler.HandleAsync(command);

    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await groupMemberRepository.DidNotReceive().AddAsync(Arg.Any<GroupMember>());
  }
}