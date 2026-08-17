using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.Expense.Request;
using ndgf.Api.Dtos.Expense.Response;
using ndgf.Api.Mappers.Expense;
using ndgf.Application.Handlers.Expense;

namespace ndgf.Api.Endpoints.Expense;

public static class ExpenseEndPoints
{
  public static IEndpointRouteBuilder MapExpenseEndPoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/groups/{groupId}/expenses", async (
      Guid groupId,
      CreateExpenseRequestDto dto, 
      CreateExpenseHandler handler, 
      ClaimsPrincipal user) =>
    {
      var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var userId = Guid.Parse(userIdClaim!);

      var command = dto.ToCommand(userId,  groupId);

      var result = await handler.HandleAsync(command);

      if (!result.IsSuccess)
      {
        return Results.BadRequest(result.ErrorMessage);
      }

      var response = result.Value!.ToResponseDto();

      return Results.Created($"/api/groups/{groupId}/expenses/{response.Id}", response);
    })
    .RequireAuthorization()
    .WithName("CreateExpense")
    .WithSummary("Créer une dépense dans un groupe")
    .WithDescription("Permet à un user membre d'un groupe de dépense, de créer une nouvelle dépense pour lui ou pour un autre membre du groupe.")
    .Produces<CreateExpenseResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);
    
    return app;
  }
}