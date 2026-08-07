using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.Group.Request;
using ndgf.Api.Dtos.Group.Response;
using ndgf.Api.Mappers.Group;
using ndgf.Application.Commands.Group;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Queries;

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

    app.MapPost("/api/groups/{groupId}/members", async (
        Guid groupId,
        AddGroupMemberRequestDto dto,
        AddGroupMemberHandler handler,
        ClaimsPrincipal user) =>
      {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var userId = Guid.Parse(userIdClaim!);

        var command = new AddGroupMemberCommand(userId, groupId, dto.SearchValue);

        var result = await handler.HandleAsync(command);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(result.ErrorMessage);
        }

        return Results.Ok();
      })
      .RequireAuthorization()
      .WithName("AddGroupMember")
      .WithSummary("Ajoute un membre à un groupe")
      .WithDescription("Ajoute un utilisateur existant (identifié par email ou pseudo) à un groupe. L'appelant doit être membre du groupe.")
      .Produces(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status401Unauthorized);

    app.MapGet("/api/groups/{groupId}", async (
      Guid groupId,
      GetGroupDetailsHandler handler,
      ClaimsPrincipal user) =>
    {
      var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var userId = Guid.Parse(userIdClaim!);

      var query = new GetGroupDetailQuery(groupId, userId);
      var result = await handler.HandleAsync(query);
      if (!result.IsSuccess)
      {
        return Results.BadRequest(result.ErrorMessage);
      }
      
      var response = result.Value!.ToResponseDto();
      
      return Results.Ok(response);
    })
    .RequireAuthorization()
    .WithName("GetGroupDetails")
    .WithSummary("Consulte les détails d'un groupe")
    .WithDescription("Retourne les informations d'un groupe et la liste de ses membres.")
    .Produces<GetGroupDetailsResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);

    return app;
  }
}