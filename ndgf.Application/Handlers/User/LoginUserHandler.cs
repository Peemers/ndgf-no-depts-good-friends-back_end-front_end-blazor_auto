using Microsoft.Extensions.Logging;
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
  IRefreshTokenRepository refreshTokenRepository,
  ILogger<LoginUserHandler> logger)
{
  public async Task<Result<LoginResult>> HandleAsync(LoginUserCommand command)
  {
    Domain.Entities.User? user = await userRepository.GetUserByEmailAsync(command.Email);
    if (user is null)
    {
      logger.LogWarning("[Echec] Tentative de connexion échouée : email inconnu ({Email})", command.Email);
      return Result<LoginResult>.Failure("Email ou mot de passe incorrect");
    }

    bool verifyPass = passwordHasher.VerifyHashedPassword(user.PasswordHash, command.Password);

    if (!verifyPass)
    {
      logger.LogWarning("[Echec] Tentative de connexion échouée : mot de passe incorrect pour ({UserId})", user.Id);
      return Result<LoginResult>.Failure("Email ou mot de passe incorrect");
    }

    string accessToken = jwtService.GenerateAccessToken(user);
    string refreshToken = jwtService.GenerateRefreshToken();
    DateTime expiryDay = DateTime.UtcNow.AddDays(14);

    RefreshToken refreshTokenEntity = RefreshToken.Create(refreshToken, user.Id, expiryDay);

    await refreshTokenRepository.AddAsync(refreshTokenEntity);

    logger.LogInformation("[Succès] Connexion réussie pour l'utilisateur ({UserId})", user.Id);

    LoginResult loginResult = new LoginResult(user, accessToken, refreshToken);
    return Result<LoginResult>.Success(loginResult);
  }
}