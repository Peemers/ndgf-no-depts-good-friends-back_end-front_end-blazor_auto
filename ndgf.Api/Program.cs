using ndgf.Api.Endpoints.User;
using ndgf.Application.Extensions;
using ndgf.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();


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

app.MapUserEndpoints();
app.UseHttpsRedirection();

app.Run();

