using Microsoft.AspNetCore.RateLimiting;

namespace ndgf.Api.Extensions;

public static class RateLimiterPolicies
{
  public static IServiceCollection AddRateLimiterPolicies(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddTokenBucketLimiter("auth", authOptions =>
      {
        authOptions.TokenLimit = 9;
        authOptions.TokensPerPeriod = 3;
        authOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(20);
      });

      options.RejectionStatusCode = 429;

      options.OnRejected = (context, cancellationToken) =>
      {
        var logger = context.HttpContext.RequestServices
          .GetRequiredService<ILogger<Program>>();

        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var path = context.HttpContext.Request.Path;

        logger.LogWarning("Rate limit dépassé pour {Ip} sur {Path}", ip, path);

        return ValueTask.CompletedTask;
      };
    });
    
    return services;
  }
}