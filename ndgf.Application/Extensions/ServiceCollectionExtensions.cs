using Microsoft.Extensions.DependencyInjection;
using ndgf.Application.Handlers.Expense;
using ndgf.Application.Handlers.Group;
using ndgf.Application.Handlers.GroupHistory;
using ndgf.Application.Handlers.Refund;
using ndgf.Application.Handlers.User;
using ndgf.Application.Interfaces.Services;
using ndgf.Application.Services;

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
    services.AddScoped<CreateExpenseHandler>();
    services.AddScoped<GetGroupExpenseHandler>();
    services.AddScoped<GetGroupBalanceHandler>();
    services.AddScoped<CreateRefundHandler>();
    services.AddScoped<GetGroupHistoryHandler>();
    services.AddScoped<SoftDeleteExpenseHandler>();
    services.AddScoped<SoftDeleteRefundHandler>();
    services.AddScoped<IBalanceCalculator, BalanceCalculator>();
    return services;
  }
}