using ndgf.Api.Endpoints.Group;
using ndgf.Api.Endpoints.User;
using ndgf.Api.Extensions;
using ndgf.Application.Extensions;
using ndgf.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
app.UseAuthentication();
app.UseAuthorization();
app.MapUserEndpoints();
app.MapGroupEndpoints();
app.UseHttpsRedirection();

app.Run();

