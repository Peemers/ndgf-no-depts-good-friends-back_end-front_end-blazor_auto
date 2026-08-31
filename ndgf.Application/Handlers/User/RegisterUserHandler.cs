using ndgf.Application.Commands.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Application.Models.User;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;

namespace ndgf.Application.Handlers.User;

public class RegisterUserHandler(
  IUserRepository userRepository, 
  IPasswordHasher passwordHasher,
  IJwtService jwtService,
  IRefreshTokenRepository refreshTokenRepository)
{
  public async Task<Result<LoginResult>> HandleAsync(RegisterUserCommand command)
  {
    bool emailAlreadyExists = await userRepository.EmailAlreadyExistsAsync(command.Email);
    if (emailAlreadyExists)
    {
      return Result<LoginResult>.Failure("Cet email est déja utilisé.");
    }
    
    bool pseudoAlreadyExists = await userRepository.PseudoAlreadyExistsAsync(command.Pseudo);
    if (pseudoAlreadyExists)
    {
      return Result<LoginResult>.Failure("Pseudo déjà utilisé");
    }

    string passwordHash = passwordHasher.HashPassword(command.Password);

    Domain.Entities.User user = Domain.Entities.User.Create(command.Email, passwordHash, command.Pseudo, command.LastName, command.FirstName);
    
    Domain.Entities.User savedUser = await userRepository.AddAsync(user);
    
    string accessToken = jwtService.GenerateAccessToken(user);
    string refreshToken = jwtService.GenerateRefreshToken();
    DateTime expiryDay = DateTime.UtcNow.AddDays(14);

    RefreshToken refreshTokenEntity = RefreshToken.Create(refreshToken, user.Id, expiryDay);

    await refreshTokenRepository.AddAsync(refreshTokenEntity);

    LoginResult loginResult = new LoginResult(savedUser, accessToken, refreshToken);
    
    return Result<LoginResult>.Success(loginResult);
    
  }
}