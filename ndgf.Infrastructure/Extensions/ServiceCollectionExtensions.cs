using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ndgf.Application.Interfaces.Repositories;
using ndgf.Application.Interfaces.Security;
using ndgf.Infrastructure.Persistence;
using ndgf.Infrastructure.Persistence.Repositories;
using ndgf.Infrastructure.Security;

namespace ndgf.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    
    services.AddDbContext<NdgfDbContext>(options =>
      options.UseSqlServer(connectionString));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    
    return services;
  }
}