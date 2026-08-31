using ndgf.Application.Commands.User;
using ndgf.Application.Handlers.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using NSubstitute;

namespace ndgf.Application.Tests.User;

public class RegisterUserHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithNewEmail_ShouldReturnSuccessResult()
  {
    // Arrange
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

    var expectedUser = Domain.Entities.User.Create("test@test.be", "hashedPassword123", "Toto", "Dupont", "Jean");

    userRepository.EmailAlreadyExistsAsync(Arg.Any<string>()).Returns(false);
    userRepository.PseudoAlreadyExistsAsync(Arg.Any<string>()).Returns(false);
    passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashedPassword123");
    userRepository.AddAsync(Arg.Any<Domain.Entities.User>()).Returns(expectedUser);
    jwtService.GenerateAccessToken(Arg.Any<Domain.Entities.User>()).Returns("test-access-token");
    jwtService.GenerateRefreshToken().Returns("test-refresh-token");

    var handler = new RegisterUserHandler(userRepository, passwordHasher, jwtService, refreshTokenRepository);
    var command = new RegisterUserCommand("test@test.be", "MonMotDePasse123", "Toto", "Jean", "Dupont");

    // Act
    var result = await handler.HandleAsync(command);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal("test@test.be", result.Value!.User.Email);
    Assert.Equal("test-access-token", result.Value.AccessToken);
    Assert.Equal("test-refresh-token", result.Value.RefreshToken);
    Assert.NotNull(result.Value);
    Assert.Equal("test@test.be", result.Value.User.Email);
  }

  [Fact]
  public async Task HandleAsync_WithExistingEmail_ShouldReturnFailureResult()
  {
    //Arrange

    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

    userRepository.EmailAlreadyExistsAsync(Arg.Any<string>()).Returns(true);

    var handler = new RegisterUserHandler(userRepository, passwordHasher, jwtService, refreshTokenRepository);
    var command = new RegisterUserCommand("test@test.be", "MonMotDePasse123", "Toto", "Jean", "Dupont");
    
    //Act
    var result = await handler.HandleAsync(command);
    
    //Assert
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    passwordHasher.DidNotReceiveWithAnyArgs().HashPassword(Arg.Any<string>());
    await userRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.User>());
    
  }
  
  [Fact]
  public async Task HandleAsync_WithExistingPseudo_ShouldReturnFailureResult()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    
    userRepository.EmailAlreadyExistsAsync(Arg.Any<string>()).Returns(false);
    userRepository.PseudoAlreadyExistsAsync(Arg.Any<string>()).Returns(true);
    
    var handler = new RegisterUserHandler(userRepository, passwordHasher, jwtService, refreshTokenRepository);
    var command = new RegisterUserCommand("test@test.be", "Test1234=", "Toto", "Jean", "Dupont");
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    
    passwordHasher.DidNotReceiveWithAnyArgs().HashPassword(Arg.Any<string>());
    await userRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Entities.User>());
  }
}