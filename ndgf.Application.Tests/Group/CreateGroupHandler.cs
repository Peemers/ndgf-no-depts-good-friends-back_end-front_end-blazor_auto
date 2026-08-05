using ndgf.Application.Commands.Group;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using NSubstitute;
namespace ndgf.Application.Tests.Group;


public class CreateGroupHandler
{
  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccessResult()
  {
    // Arrange
    var groupRepository = Substitute.For<IGroupRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();

    groupRepository.AddAsync(Arg.Any<Domain.Entities.Group>())!
      .Returns(callInfo => callInfo.Arg<Domain.Entities.Group>());

    groupMemberRepository.AddAsync(Arg.Any<Domain.Entities.GroupMember>())!
      .Returns(callInfo => callInfo.Arg<Domain.Entities.GroupMember>());

    var handler = new CreateGroupeHandler(groupRepository, groupMemberRepository);
    var userId = Guid.NewGuid();
    var command = new CreateGroupCommand("Voyage à Tignes", "Dépenses partagées du ski-trip", userId);

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("Voyage à Tignes", result.Value.Group.Name);
    Assert.Equal(userId, result.Value.GroupMember.UserId);
    Assert.Equal(result.Value.Group.Id, result.Value.GroupMember.GroupId);
  }
}