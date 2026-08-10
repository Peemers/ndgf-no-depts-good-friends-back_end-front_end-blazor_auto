using Microsoft.Extensions.DependencyInjection;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Handlers.User;

namespace ndgf.Application.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<RegisterUserHandler>();
    services.AddScoped<LoginUserHandler>();
    services.AddScoped<CreateGroupeHandler>();
    services.AddScoped<AddGroupMemberHandler>();
    services.AddScoped<GetGroupDetailsHandler>();
    services.AddScoped<GetUserGroupsHandler>();
    return services;
  }
}