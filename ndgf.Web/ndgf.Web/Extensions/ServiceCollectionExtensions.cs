using Microsoft.AspNetCore.Components.Authorization;
using ndgf.Web.Services.Auth;
using ndgf.Web.Services.User;

namespace ndgf.Web.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddWebService(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddScoped<CustomAuthenticationStateProvider>();
    services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
    services.AddAuthorizationCore();
    services.AddScoped<AuthTokenHandler>();
    
    services.AddHttpClient<UserApiClient>(client =>
    {
      client.BaseAddress = new Uri("http://localhost:5217");
    })
    .AddHttpMessageHandler<AuthTokenHandler>();

    return services;
  }
}