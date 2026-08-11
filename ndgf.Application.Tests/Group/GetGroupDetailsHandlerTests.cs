using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Models.Group;
using ndgf.Application.Queries.Group;
using ndgf.Domain.Entities;

namespace ndgf.Application.Tests.Group;
using NSubstitute;

public class GetGroupDetailsHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccesResult()
  {
    var groupRepository = Substitute.For<IGroupRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    var userRepository = Substitute.For<IUserRepository>();
    
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();

    var existingGroup = Domain.Entities.Group.Create("Tignes", "Tignes 2026");
    var existingGroupMember = GroupMember.Create(userId, groupId);
    var existingUser = Domain.Entities.User.Create("test@test.be", "password123", "totin", "jack", "black");
    var groupMembers = new List<GroupMember> { existingGroupMember };
    
    
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns(existingGroup);
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>()).Returns(groupMembers);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
    userRepository.GetUserByIdAsync(Arg.Any<Guid>()).Returns(existingUser);
    
    var handler = new GetGroupDetailsHandler(groupRepository, groupMemberRepository, userRepository);
    var query = new GetGroupDetailQuery(groupId, userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.NotNull(result);
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("Tignes", result.Value.Group.Name);
    var memberInfo = Assert.Single(result.Value.Members); //regarde si la liste se rempli et renvoi pour les Assert en dessous.
    Assert.Equal(existingUser.Id, memberInfo.UserId);
    Assert.Equal(existingUser.Pseudo, memberInfo.Pseudo);
    Assert.Equal(existingUser.Email, memberInfo.Email);
  }
  
  [Fact]
  public async Task HandleAsync_WithUnexistingGroup_ShouldReturnFailureResult()
  {
    var groupRepository = Substitute.For<IGroupRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    var userRepository = Substitute.For<IUserRepository>();
    
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns((Domain.Entities.Group?)null);
    
    var handler = new GetGroupDetailsHandler(groupRepository, groupMemberRepository, userRepository);
    var query = new GetGroupDetailQuery(groupId, userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await groupMemberRepository.DidNotReceive().GetMemberByGroupIdAsync(Arg.Any<Guid>());
    await userRepository.DidNotReceive().GetUserByIdAsync(Arg.Any<Guid>());
  }
  
  [Fact]
  public async Task HandleAsync_WithMemberNotInGroup_ShouldReturnFailureResult()
  {
    var groupRepository = Substitute.For<IGroupRepository>();
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    var userRepository = Substitute.For<IUserRepository>();
    
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var existingGroup = Domain.Entities.Group.Create("Tignes", "Tignes 2026");
    
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns(existingGroup);
    groupMemberRepository.IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false, false);
    
    var handler = new GetGroupDetailsHandler(groupRepository, groupMemberRepository, userRepository);
    var query = new GetGroupDetailQuery(groupId, userId);
    var result = await handler.HandleAsync(query);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await groupMemberRepository.DidNotReceive().GetMemberByGroupIdAsync(Arg.Any<Guid>());
  }
}