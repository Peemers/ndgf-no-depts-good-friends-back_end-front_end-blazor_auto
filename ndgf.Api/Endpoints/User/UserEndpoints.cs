using ndgf.Api.Dtos.User.Request;
using ndgf.Api.Dtos.User.Response;
using ndgf.Api.Mappers;
using ndgf.Api.Mappers.User;
using ndgf.Application.Handlers.User;

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
      .Produces<RegisterUserResponseDto>(StatusCodes.Status201Created)
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


    return app;
  }
}