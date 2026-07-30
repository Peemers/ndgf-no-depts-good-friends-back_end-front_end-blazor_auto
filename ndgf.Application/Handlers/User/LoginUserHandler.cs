using ndgf.Application.Commands.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Application.Models.User;
using ndgf.Domain.Common;
using ndgf.Domain.Entities;

namespace ndgf.Application.Handlers.User;

public class LoginUserHandler(
  IUserRepository userRepository,
  IJwtService jwtService,
  IPasswordHasher passwordHasher,
  IRefreshTokenRepository refreshTokenRepository)
{
  public async Task<Result<LoginResult>> HandleAsync(LoginUserCommand command)
  {
    Domain.Entities.User? user = await userRepository.GetUserByEmailAsync(command.Email);
    if (user is null)
    {
      return Result<LoginResult>.Failure("Email ou mot de passe incorrect");
    }

    bool verifyPass = passwordHasher.VerifyHashedPassword(user.PasswordHash, command.Password);

    if (!verifyPass)
    {
      return Result<LoginResult>.Failure("Email ou mot de passe incorrect");
    }

    string accessToken = jwtService.GenerateAccessToken(user);
    string refreshToken = jwtService.GenerateRefreshToken();
    DateTime expiryDay = DateTime.UtcNow.AddDays(7);

    RefreshToken refreshTokenEntity = RefreshToken.Create(refreshToken, user.Id, expiryDay);

    await refreshTokenRepository.AddAsync(refreshTokenEntity);

    LoginResult loginResult = new LoginResult(user, accessToken, refreshToken);
    return Result<LoginResult>.Success(loginResult);
  }
}