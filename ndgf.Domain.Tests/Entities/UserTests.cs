using ndgf.Domain.Entities;
using ndgf.Domain.Exceptions;

namespace ndgf.Domain.Tests.Entities;

public class UserTests
{
  [Fact]
  public void Create_WithValidData_ShouldCreateUser()
  {
    //Arrange
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var firstName = "Jean";
    var lastName = "Dupont";
    
    //Act
    
    var user = User.Create(email, passwordHash, pseudo, lastName, firstName);
    
    //Assert
    
    Assert.Equal(email, user.Email);
    Assert.Equal(passwordHash, user.PasswordHash);
    Assert.Equal(pseudo, user.Pseudo);
    Assert.Equal(firstName, user.FirstName);
    Assert.Equal(lastName, user.LastName);
    Assert.NotEqual(Guid.Empty, user.Id);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("emailinvalide")]
  [InlineData("email@")]
  [InlineData("@test.com")]
  public void Create_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
  {
    //Arrange
    
    var passwordHash = "test123";
    var pseudo = "Toto";
    var firstName = "Jean";
    var lastName = "Dupont";
    
    //Act & Assert
    
    Assert.Throws<DomainException>(() => User.Create(invalidEmail, passwordHash, pseudo, lastName, firstName));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithInvalidPasswordHash_ShouldThrowDomainException(string invalidPasswordHash)
  {
    var pseudo = "Toto";
    var firstName = "Jean";
    var lastName = "Dupont";
    var email = "test@test.be";
    
    Assert.Throws<DomainException>(() => User.Create(email, invalidPasswordHash, pseudo, lastName, firstName));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithInvalidPseudo_ShouldThrowDomainException(string invalidPseudo)
  {
    var firstName = "Jean";
    var lastName = "Dupont";
    var email = "test@test.be";
    var passwordHash = "test123";
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, invalidPseudo, lastName, firstName));
  }
  
  [Fact]
  public void Create_WithTooLongPseudo_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var tooLongPseudo = new string('a', 31);
    var firstName = "Jean";
    var lastName = "Dupont";
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, tooLongPseudo, lastName, firstName));
  }

  [Fact]
  public void Create_WithTooShortPseudo_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var tooShortPseudo = new string('a', 1);
    var firstName = "Jean";
    var lastName = "Dupont";
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, tooShortPseudo, lastName, firstName));
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithInvalidFirstName_ShouldThrowDomainException(string invalidFirstName)
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var lastName = "Dupont";
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, lastName, invalidFirstName));
  }

  [Fact]
  public void Create_WithTooLongFirstName_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var lastName = "Dupont";
    var toLongFirstName = new string('a', 31);
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, lastName, toLongFirstName));
  }
  
  [Fact]
  public void Create_WithTooShortFirstName_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var lastName = "Dupont";
    var toShortFirstName = new string('a', 1);
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, lastName, toShortFirstName));
  }
  
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Create_WithInvalidLastName_ShouldThrowDomainException(string invalidLastName)
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var firstName = "Dupont";
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, invalidLastName, firstName));
  }

  [Fact]
  public void Create_WithTooLongLastName_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var firstName = "Dupont";
    var toLongLastName = new string('a', 31);
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, toLongLastName, firstName ));
  }
  
  [Fact]
  public void Create_WithTooShortLastName_ShouldThrowDomainException()
  {
    var email = "test@test.be";
    var passwordHash = "test123";
    var pseudo = "Toto";
    var firstName = "Jean";
    var toShortLastName = new string('a', 1);
    
    Assert.Throws<DomainException>(() => User.Create(email, passwordHash, pseudo, toShortLastName, firstName));
  }
}