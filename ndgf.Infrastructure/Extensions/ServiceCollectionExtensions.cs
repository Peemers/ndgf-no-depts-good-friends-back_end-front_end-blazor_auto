using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Application.Interfaces.Services;
using ndgf.Infrastructure.Persistence;
using ndgf.Infrastructure.Persistence.Repositories;
using ndgf.Infrastructure.Security;
using ndgf.Infrastructure.Services;

namespace ndgf.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    
    services.AddHttpClient<IGeoCodingService, GeoCodingService>(client =>
    {
      client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
      client.DefaultRequestHeaders.Add("User-Agent", "NDGF-App/1.0");
    });
    
    services.AddDbContext<NdgfDbContext>(options =>
      options.UseSqlServer(connectionString));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddTransient<IJwtService, JwtService>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IGroupRepository, GroupRepository>();
    services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
    services.AddScoped<IExpenseRepository, ExpenseRepository>();
    services.AddScoped<IRefundRepository, RefundRepository>();
    
    return services;
  }
}