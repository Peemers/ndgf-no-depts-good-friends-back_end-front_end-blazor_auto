using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class GroupTests
{
  [Fact]
  public void Create_WithValidData_ShouldCreateGroup()
  {
    //Arrange

    var name = "Test";
    var description = "Test description";

    //Act

    var group = Group.Create(name, description);

    Assert.Equal(name, group.Name);
    Assert.Equal(description, group.Description);
    Assert.NotEqual(Guid.Empty, group.Id);
    Assert.True(group.CreatedAt <= DateTime.UtcNow);
  }
  
  [Fact]
  public void Create_WithValidDataWithoutDescription_ShouldCreateGroup()
  {
    var name = "Test";
    
    
    var group = Group.Create(name,  null);
    
    Assert.Equal(name, group.Name);
    Assert.NotEqual(Guid.Empty, group.Id);
    Assert.True(group.CreatedAt <= DateTime.UtcNow);
    Assert.Null(group.Description);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithNameWhiteOrEmpty_ShouldThrowDomainException(string invalidEmail)
  {
    //Arrange
    
    var description = "Test description";
    
    //Act & Assert
    
    Assert.Throws<DomainException>(() => Group.Create(invalidEmail, description));
  }

  [Fact]
  public void Create_WithNameTooShort_ShouldThrowDomainException()
  {
    //Arrange
    
    var name = new string('x', 2);
    var description = "Test description";
    
    //Act & Assert
    
    Assert.Throws<DomainException>(() => Group.Create(name, description));
  }

  [Fact]
  public void Create_WithNameToLong_ShouldThrowDomainException()
  {
    //Arrange
    
    var name = new string('x', 200);
    var description = "Test description";
    
    //Act & Assert
    
    Assert.Throws<DomainException>(() => Group.Create(name, description));
  }
  
  [Fact]
  public void Create_WithDescriptionTooShortWhenProvided_ShouldThrowDomainException()
  {
    //Arrange
    
    var name = "Test";
    var description = new string('x', 2);
    
    //Act & Assert
    
    Assert.Throws<DomainException>(() => Group.Create(name, description));
  }

  [Fact]
  public void Create_WithDescriptionTooLongWhenProvided_ShouldThrowDomainException()
  {
    //Arrange
    var name = "Test";
    var description = new string('x', 300);
    
    //Act & Assert
    Assert.Throws<DomainException>(() => Group.Create(name, description));
  }

  [Fact]
  public void ArchiveGroup_OnUnArchivedGroup_ShouldSetArchivedAt()
  {
    var name = "Test";
    var description = "Test description";
    var group = Group.Create(name, description);
    
    group.Archive();
    
    Assert.NotNull(group.ArchivedAt);
  }

  [Fact]
  public void ArchiveGroup_OnArchivedGroup_ThrowDomainException()
  {
    var name = "Test";
    var description = "Test description";
    var group = Group.Create(name, description);
    
    group.Archive();
    
    Assert.Throws<DomainException>(() => group.Archive());
  }
  
}