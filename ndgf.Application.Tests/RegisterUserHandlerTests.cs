using ndgf.Application.Commands.User;
using ndgf.Application.Handlers.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using NSubstitute;

namespace ndgf.Application.Tests;

public class RegisterUserHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithNewEmail_ShouldReturnSuccessResult()
  {
    // Arrange
    var userRepository = Substitute.For<IUserRepository>();
    var passwordHasher = Substitute.For<IPasswordHasher>();

    var expectedUser = Domain.Entities.User.Create("test@test.be", "hashedPassword123", "Toto", "Dupont", "Jean");

    userRepository.EmailAlreadyExistsAsync(Arg.Any<string>()).Returns(false);
    passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword123");
    userRepository.AddAsync(Arg.Any<Domain.Entities.User>()).Returns(expectedUser);

    var handler = new RegisterUserHandler(userRepository, passwordHasher);
    var command = new RegisterUserCommand("test@test.be", "MonMotDePasse123", "Toto", "Jean", "Dupont");

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("test@test.be", result.Value.Email);
  }

  [Fact]
  public async Task HandleAsync_WithExistingEmail_ShouldReturnFailureResult()
  {
    //Arrange

    var userRepository = Substitute.For<IUserRepository>();
    var passwordHasher = Substitute.For<IPasswordHasher>();

    userRepository.EmailAlreadyExistsAsync(Arg.Any<string>()).Returns(true);

    var handler = new RegisterUserHandler(userRepository, passwordHasher);
    var command = new RegisterUserCommand("test@test.be", "MonMotDePasse123", "Toto", "Jean", "Dupont");
    
    //Act
    var result = await handler.HandleAsync(command);
    
    //Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    userRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.User>());
  }
}