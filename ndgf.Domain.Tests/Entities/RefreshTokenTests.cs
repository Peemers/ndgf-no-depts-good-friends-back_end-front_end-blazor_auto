using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class RefreshTokenTests
{
  [Fact]
  public void CreateRefreshToken_WithValidData_ShouldCreateRefreshToken()
  {
    //Arrange
    var userNewId = Guid.NewGuid();

    var token = "some-refresh-token";
    var userId = userNewId;
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    
    //Act
    var refreshToken = Domain.Entities.RefreshToken.Create(token, userId, expiresAt);
    
    //Assert
    Assert.Equal(token, refreshToken.Token);
    Assert.Equal(userId, refreshToken.UserId);
    Assert.Equal(expiresAt, refreshToken.ExpiresAt);
    Assert.Null(refreshToken.RevokedAt);
    Assert.NotEqual(Guid.Empty, refreshToken.UserId);
    Assert.True(refreshToken.CreatedAt <= DateTime.UtcNow);
  }
  
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void CreateRefreshToken_WithInvalidAccessToken_ShouldThrowDomainException(string invalidToken)
  {
    var userId = Guid.NewGuid();
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    
    Assert.Throws<DomainException>(() => Domain.Entities.RefreshToken.Create(invalidToken, userId, expiresAt));
  }

  [Fact]
  public void CreateRefreshToken_WithInvalidUserId_ShouldThrowDomainException()
  {
    //Arrange
    var token = "some-refresh-token";
    var userId = Guid.Empty;
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    
    //Act
    //Assert
    Assert.Throws<DomainException>(() => Domain.Entities.RefreshToken.Create(token, userId, expiresAt));
  }

  [Fact]
  //Arrange
  public void CreateRefreshToken_WithInvalidExpiresAt_ShouldThrowDomainException()
  {
    var token = "some-refresh-token";
    var userId = Guid.NewGuid();
    var expiresAt = DateTime.UtcNow.AddMinutes(-30);
    
    //Act
    //Assert
    Assert.Throws<DomainException>(() => Domain.Entities.RefreshToken.Create(token, userId, expiresAt));
  }

  [Fact]
  public void RevokeRefreshToken_OnUnrevokedToken_ShouldThrowDomainException()
  {
    //Arrange
    var token = "some-refresh-token";
    var userId = Guid.NewGuid();
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    var refreshToken = Domain.Entities.RefreshToken.Create(token, userId, expiresAt);
    
    //Act
    refreshToken.Revoke();
    
    //Assert
    Assert.NotNull(refreshToken.RevokedAt);
  }

  [Fact]
  public void RevokeRefreshToken_OnRevokedToken_ShouldSetRevokedAt()
  {
    //Arrange
    var token = "some-refresh-token";
    var userId = Guid.NewGuid();
    var expiresAt = DateTime.UtcNow.AddMinutes(30);
    var refreshToken = Domain.Entities.RefreshToken.Create(token, userId, expiresAt);
    
    //Act
    refreshToken.Revoke();
    
    //Assert
    Assert.Throws<DomainException>(() => refreshToken.Revoke());
  }
}