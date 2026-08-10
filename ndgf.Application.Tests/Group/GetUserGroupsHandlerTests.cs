using ndgf.Application.Handlers.Group;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Queries.Group;
using ndgf.Domain.Entities;

namespace ndgf.Application.Tests.Group;
using NSubstitute;

public class GetUserGroupsHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidCommand_ShouldReturnSuccessResult()
  {
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    var groupRepository = Substitute.For<IGroupRepository>();
    
    var userId = Guid.NewGuid();
    var groupId = Guid.NewGuid();

    var existingGroup = Domain.Entities.Group.Create("Tignes 2031", "Description");
    var existingGroupMember = GroupMember.Create(userId, groupId);
    var groupMembers = new List<GroupMember> { existingGroupMember };
    
    groupRepository.GetGroupByIdAsync(Arg.Any<Guid>()).Returns(existingGroup);
    
    groupMemberRepository.GetMemberByGroupIdAsync(Arg.Any<Guid>()).Returns(groupMembers);
    groupMemberRepository.GetMembershipsByUserIdAsync(Arg.Any<Guid>()).Returns(groupMembers);
    
    var handler = new GetUserGroupsHandler(groupMemberRepository, groupRepository);
    var query = new GetUserGroupsQuery(userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.NotNull(result);
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    var groupsInfo = Assert.Single(result.Value.Groups);
    Assert.Equal(existingGroup.Id, groupsInfo.Id);
    Assert.Equal(existingGroup.Name, groupsInfo.Name);
    Assert.Equal(existingGroup.Description, groupsInfo.Description);
    Assert.Equal(1, groupsInfo.MemberCount);
  }
  
  [Fact]
  public async Task HandleAsync_WithUserWithoutGroup_ShouldReturnSuccessResult()
  {
    var groupMemberRepository = Substitute.For<IGroupMemberRepository>();
    var groupRepository = Substitute.For<IGroupRepository>();
    
    var userId = Guid.NewGuid();
    
    groupMemberRepository.GetMembershipsByUserIdAsync(Arg.Any<Guid>()).Returns(new List<GroupMember>());
    
    var handler = new GetUserGroupsHandler(groupMemberRepository, groupRepository);
    var query = new GetUserGroupsQuery(userId);
    
    var result = await handler.HandleAsync(query);
    
    Assert.Empty(result.Value!.Groups);
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
  }
}