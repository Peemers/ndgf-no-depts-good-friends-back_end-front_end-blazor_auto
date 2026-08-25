using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.GroupHistory.Response;
using ndgf.Api.Mappers.GroupHistory;
using ndgf.Application.Handlers.GroupHistory;
using ndgf.Application.Queries.GroupHistory;

namespace ndgf.Api.Endpoints.GroupHistory;

public static class GroupHistoryEndPoint
{
  public static IEndpointRouteBuilder MapGroupHistoryEndpoint(this IEndpointRouteBuilder app)
  {
    app.MapGet("/api/groups/{groupId}/history", async (
      Guid groupId,
      int page,
      int pageSize,
      bool sortDescending,
      GetGroupHistoryHandler handler,
      ClaimsPrincipal user) =>
    {
      var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var userId = Guid.Parse(userIdClaim!);

      var query = new GetGroupHistoryQuery(groupId, userId, page, pageSize, sortDescending);
      var result = await handler.HandleAsync(query);

      if (!result.IsSuccess)
      {
        return Results.BadRequest(result.ErrorMessage);
      }

      var response = result.Value!.ToResponseDto();

      return Results.Ok(response);
    })
    .RequireAuthorization()
    .WithName("GetGroupHistory")
    .WithSummary("Consulte les dépenses ET les remboursements paginées d'un groupe")
    .WithDescription("Retourne une liste paginée des dépenses ET les remboursement d'un groupe, triée par date.")
    .Produces<GetGroupHistoryResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);
    
    return app;
  }
}