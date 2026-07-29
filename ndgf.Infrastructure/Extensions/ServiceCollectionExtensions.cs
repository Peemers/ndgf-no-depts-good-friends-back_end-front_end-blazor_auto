using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ndgf.Infrastructure.Persistence;

namespace ndgf.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    
    services.AddDbContext<NdgfDbContext>(options =>
      options.UseSqlServer(connectionString));
    
    return services;
  }
}