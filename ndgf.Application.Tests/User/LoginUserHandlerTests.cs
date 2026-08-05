using ndgf.Application.Commands.User;
using ndgf.Application.Handlers.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Domain.Entities;
using NSubstitute;

namespace ndgf.Application.Tests.User;

public class LoginUserHandlerTests
{
  [Fact]
  public async Task HandleAsync_WithValidCredentials_ReturnsSuccess()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();

    var expectedUser = Domain.Entities.User.Create("test@test.be", "hashedPassword123", "toto", "Doe", "John");
    
    
    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(expectedUser);
    passwordHasher.VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
    jwtService.GenerateAccessToken(Arg.Any<Domain.Entities.User>()).Returns("fakeAccessToken");
    jwtService.GenerateRefreshToken().Returns("fakeRefreshToken");
    
    var handler = new LoginUserHandler(userRepository, jwtService, passwordHasher, refreshTokenRepository);
    var command = new LoginUserCommand("test@test.be", "monMotDePasse123");
    
    var result = await handler.HandleAsync(command);
    
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal("fakeAccessToken", result.Value.AccessToken);
    Assert.Equal("fakeRefreshToken", result.Value.RefreshToken);
    Assert.Equal(expectedUser.Email, result.Value.User.Email);
  }
  
  [Fact]
  public async Task HandleAsync_WithInvalidEmail_ReturnsFailure()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    
    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns((Domain.Entities.User?)null);
    
    var handler =  new LoginUserHandler(userRepository, jwtService, passwordHasher, refreshTokenRepository);
    var command = new LoginUserCommand("test@test.be", "monMotDePasse123");
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await userRepository.Received(1).GetUserByEmailAsync(Arg.Any<string>());
    passwordHasher.DidNotReceive().VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>());
    jwtService.DidNotReceive().GenerateAccessToken(Arg.Any<Domain.Entities.User>());
    await refreshTokenRepository.DidNotReceive().AddAsync(Arg.Any<RefreshToken>());
  }

  [Fact]
  public async Task HandleAsync_WithInvalidPassword_ReturnsFailure()
  {
    IUserRepository userRepository = Substitute.For<IUserRepository>();
    IJwtService jwtService = Substitute.For<IJwtService>();
    IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    IRefreshTokenRepository refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    
    var expectedUser = Domain.Entities.User.Create("test@test.be", "hashedPassword123", "toto", "Doe", "John");
    
    userRepository.GetUserByEmailAsync(Arg.Any<string>()).Returns(expectedUser);
    passwordHasher.VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
    
    var handler = new LoginUserHandler(userRepository, jwtService, passwordHasher, refreshTokenRepository);
    var command = new LoginUserCommand("test@test.be", "monMotDePasse123");
    
    var result = await handler.HandleAsync(command);
    
    Assert.False(result.IsSuccess);
    Assert.NotNull(result.ErrorMessage);
    await userRepository.Received(1).GetUserByEmailAsync(Arg.Any<string>());
    passwordHasher.Received(1).VerifyHashedPassword(Arg.Any<string>(), Arg.Any<string>());
    jwtService.DidNotReceive().GenerateAccessToken(Arg.Any<Domain.Entities.User>());
    await refreshTokenRepository.DidNotReceive().AddAsync(Arg.Any<RefreshToken>());
  }
}