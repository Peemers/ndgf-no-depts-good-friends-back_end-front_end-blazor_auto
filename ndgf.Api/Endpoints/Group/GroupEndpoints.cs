using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.Group.Request;
using ndgf.Api.Dtos.Group.Response;
using ndgf.Api.Mappers.Group;
using ndgf.Application.Handlers.Group;

namespace ndgf.Api.Endpoints.Group;

public static class GroupEndpoints
{
  public static IEndpointRouteBuilder MapGroupEndpoints(this IEndpointRouteBuilder app)
  {
    //ClaimsPrincipal user = pour l'id du token.
    app.MapPost("/api/groups", async (CreateGroupRequestDto dto, CreateGroupeHandler handler, ClaimsPrincipal user) =>
      {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var userId = Guid.Parse(userIdClaim!);

        var command = dto.ToCommand(userId);

        var result = await handler.HandleAsync(command);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(result.ErrorMessage);
        }

        var response = result.Value!.ToResponseDto();

        return Results.Created($"/api/groups/{response.Id}", response);
      })
      .RequireAuthorization()
      .WithName("CreateGroup")
      .WithSummary("Crée un nouveau groupe")
      .WithDescription("Crée un groupe et ajoute automatiquement le créateur comme membre. Nécessite d'être authentifié.")
      .Produces<CreateGroupResponseDto>(StatusCodes.Status201Created)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status401Unauthorized);

    return app;
  }
}