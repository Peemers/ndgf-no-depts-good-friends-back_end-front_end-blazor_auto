using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ndgf.Api.Dtos.Expense.Request;
using ndgf.Api.Dtos.Expense.Response;
using ndgf.Api.Mappers.Expense;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Queries.Expense;

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
    
    app.MapGet("/api/groups/{groupId}/expenses", async (
        Guid groupId,
        int page,
        int pageSize,
        bool sortDescending,
        GetGroupExpenseHandler handler,
        ClaimsPrincipal user) =>
      {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var userId = Guid.Parse(userIdClaim!);

        var query = new GetGroupExpenseQuery(groupId, userId, page, pageSize, sortDescending);

        var result = await handler.HandleAsync(query);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(result.ErrorMessage);
        }

        var response = result.Value!.ToResponseDto();

        return Results.Ok(response);
      })
      .RequireAuthorization()
      .WithName("GetGroupExpenses")
      .WithSummary("Consulte les dépenses paginées d'un groupe")
      .WithDescription("Retourne une liste paginée des dépenses d'un groupe, triée par date.")
      .Produces<GetGroupExpensesResponseDto>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status400BadRequest)
      .Produces(StatusCodes.Status401Unauthorized);

    app.MapGet("/api/groups/{groupId}/balance", async (
      Guid groupId,
      GetGroupBalanceHandler getGroupBalanceHandler,
      ClaimsPrincipal user) =>
    {
      var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var userId = Guid.Parse(userIdClaim!);
      
      var query = new GetGroupBalanceQuery(groupId, userId);
      
      var result = await getGroupBalanceHandler.HandleAsync(query);

      if (!result.IsSuccess)
      {
        return Results.BadRequest(result.ErrorMessage);
      }

      var response = result.Value!.ToResponseDto();
      
      return Results.Ok(response);
    })
    .RequireAuthorization()
    .WithName("GetGroupBalance")
    .WithSummary("Consulte la balance des users du groupe")
    .WithDescription("Retourne la balance d'un groupe")
    .Produces<GetGroupBalanceResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized);
    
    return app;
  }
}