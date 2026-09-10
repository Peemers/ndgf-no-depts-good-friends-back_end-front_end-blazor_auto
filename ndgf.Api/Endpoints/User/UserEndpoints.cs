using ndgf.Api.Dtos.User.Request;
using ndgf.Api.Dtos.User.Response;
using ndgf.Api.Mappers.User;
using ndgf.Application.Handlers.User;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Domain.Entities;

namespace ndgf.Api.Endpoints.User;

public static class UserEndpoints
{
  public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/users/register", async (RegisterUserRequestDto dto, RegisterUserHandler handler) =>
      {
        var command = dto.ToCommand();

        var result = await handler.HandleAsync(command);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(result.ErrorMessage);
        }

        var response = result.Value!.ToResponseDto();

        return Results.Created($"/api/users/{response.Id}", response);
      })
      .WithName("RegisterUser")
      .WithSummary("Inscrit un nouvel utilisateur")
      .WithDescription("Crée un compte utilisateur avec email, mot de passe et informations personnelles")
      .Produces<LoginUserResponseDto>(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status400BadRequest);

    app.MapPost("/api/users/login", async (LoginUserRequestDto dto, LoginUserHandler handler) =>
      {
        var command = dto.ToCommand();

        var result = await handler.HandleAsync(command);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(result.ErrorMessage);
        }

        var response = result.Value!.ToResponseDto();

        return Results.Ok(response);
      })
      .WithName("LoginUser")
      .WithSummary("Connexion utilisateur")
      .WithDescription("Connexion de l'utilisateur avec email et mot de passe et protection jwt")
      .Produces<LoginUserResponseDto>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest);
    
    app.MapPost("/api/users/refresh", async (
        RefreshTokenRequestDto dto,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService,
        ILogger<Program> logger) =>
      {
        var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(dto.RefreshToken);

        if (storedToken is null)
        {
          logger.LogWarning("Tentative de refresh avec un refresh-token inconnu.");
          return Results.Unauthorized();
        }

        if (storedToken.RevokedAt is not null)
        {
          logger.LogWarning("Tentative de refresh avec un refresh-token révoqué !");
          return Results.Unauthorized();
        }

        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
          logger.LogInformation("Tentative de refresh avec un refresh-token expiré.");
          return Results.Unauthorized();
        }

        var user = await userRepository.GetUserByIdAsync(storedToken.UserId);
        if (user is null)
        {
          return Results.Unauthorized();
        }

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshTokenValue = jwtService.GenerateRefreshToken();

        storedToken.Revoke();
        await refreshTokenRepository.UpdateAsync(storedToken);

        var newRefreshToken = RefreshToken.Create(newRefreshTokenValue, user.Id, DateTime.UtcNow.AddDays(14));
        await refreshTokenRepository.AddAsync(newRefreshToken);

        var response = new RefreshTokenResponseDto
        {
          AccessToken = newAccessToken,
          RefreshToken = newRefreshTokenValue
        };

        return Results.Ok(response);
      })
      .AllowAnonymous()
      .WithName("RefreshToken")
      .WithSummary("Renouvelle un access token à partir d'un refresh token")
      .WithDescription("Vérifie la validité du refresh token fourni, révoque l'ancien, génère et retourne une nouvelle paire de tokens")
      .Produces<RefreshTokenResponseDto>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status401Unauthorized);


    return app;
  }
}