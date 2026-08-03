using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class GroupMemberTest
{
  [Fact]
  public void CreateWithValidData_ShouldCreateGroupMember()
  {
    //Arrange
    
    var groupId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    
    //Act
    
    var groupMember = GroupMember.Create(userId, groupId);
    
    //Assert
    
    Assert.Equal(userId, groupMember.UserId);
    Assert.Equal(groupId, groupMember.GroupId);
    Assert.True(groupMember.JoinedAt <= DateTime.UtcNow);
    Assert.NotEqual(Guid.Empty, groupMember.GroupId);
    Assert.NotEqual(Guid.Empty, groupMember.UserId);
  }
  
  [Fact]
  public void CreateWithInvalidGroupId_ShouldThrowException()
  {
    
    var groupId = Guid.Empty;
    var userId = Guid.NewGuid();
    
    Assert.Throws<DomainException>(() => GroupMember.Create(userId, groupId));
  }

  [Fact]
  public void CreateWithInvalidUserId_ShouldThrowException()
  {
    var groupId = Guid.NewGuid();
    var userId = Guid.Empty;
    
    Assert.Throws<DomainException>(() => GroupMember.Create(userId, groupId));
  }
}