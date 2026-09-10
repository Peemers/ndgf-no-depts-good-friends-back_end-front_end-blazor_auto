using Microsoft.EntityFrameworkCore;
using ndgf.Api.Endpoints.Expense;
using ndgf.Api.Endpoints.Group;
using ndgf.Api.Endpoints.GroupHistory;
using ndgf.Api.Endpoints.Refund;
using ndgf.Api.Endpoints.User;
using ndgf.Api.Extensions;
using ndgf.Api.Middlewares;
using ndgf.Application.Extensions;
using ndgf.Infrastructure.Extensions;
using ndgf.Infrastructure.Logging;
using ndgf.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference(option =>
  {
    option.Title = "NDGF API";
    option.Theme = ScalarTheme.Moon;
  });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.MapGroupEndpoints();
app.MapRefundEndPoints();
app.MapExpenseEndPoints();
app.MapGroupHistoryEndpoint();
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
  try
  {
    var dbContext = scope.ServiceProvider.GetRequiredService<NdgfDbContext>();
    dbContext.Database.Migrate();
  }
  catch (Exception ex)
  {
    Log.Fatal(ex, "Échec de la migration de la base de données au démarrage");
    throw;
  }
}

app.Run();

