using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.Refund.Request;
using ndgf.Api.Dtos.Refund.Response;
using ndgf.Api.Mappers.Refund;
using ndgf.Application.Handlers.Refund;

namespace ndgf.Api.Endpoints.Refund;

public static class RefundEndPoints
{
  public static IEndpointRouteBuilder MapRefundEndPoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/groups/{groupId}/refunds", async (
    Guid groupId,
    CreateRefundRequestDto dto, 
    CreateRefundHandler createRefundHandler, 
    ClaimsPrincipal user) =>
    {
      var userClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var userId = Guid.Parse(userClaim!);
      
      var command = dto.ToCommand(userId,  groupId);
      
      var result = await createRefundHandler.HandleAsync(command);

      if (!result.IsSuccess)
      {
        return Results.BadRequest(result.ErrorMessage);
      }

      var response = result.Value!.ToResponseDto();
      
      return Results.Created($"/api/groups/{groupId}/refunds/{response.Id}", response);
    })
    .RequireAuthorization()
    .WithName("CreateRefund")
    .WithSummary("Crée un nouveau remboursement")
    .WithDescription("Permet aux utilisateurs de soldé leur(s) créance(s)")
    .Produces<CreateRefundResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);
    
    return app;
  }
}