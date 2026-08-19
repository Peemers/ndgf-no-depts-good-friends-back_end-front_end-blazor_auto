using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using ndgf.Web.Services.Auth;
using ndgf.Web.Services.Expense;
using ndgf.Web.Services.Group;
using ndgf.Web.Services.User;

namespace ndgf.Web.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddWebService(this IServiceCollection services, IConfiguration configuration)
  {
    const string apiUri = "http://localhost:5217";

    services.AddScoped<CustomAuthenticationStateProvider>();
    services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

    services.AddAuthentication(options =>
      {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie(options =>
      {
        options.LoginPath = "/login";
      });

    services.AddAuthorizationCore();
    services.AddHttpContextAccessor();
    services.AddScoped<AuthTokenHandler>();

    services.AddHttpClient<UserApiClient>(client =>
      {
        client.BaseAddress = new Uri(apiUri);
      })
      .AddHttpMessageHandler<AuthTokenHandler>();
    
    services.AddHttpClient<ExpenseApiClient>(client =>
    {
      client.BaseAddress = new Uri(apiUri);
    })
    .AddHttpMessageHandler<AuthTokenHandler>();

    services.AddHttpClient<GroupApiClient>(client =>
      {
        client.BaseAddress = new Uri(apiUri);
      })
      .AddHttpMessageHandler<AuthTokenHandler>();

    return services;
  }
}