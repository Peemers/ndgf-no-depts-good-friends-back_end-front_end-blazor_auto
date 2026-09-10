using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ndgf.Infrastructure.Logging;

public static class WebApplicationBuilderExtensions
{
  public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
  {
    var seqServerUrl = builder.Configuration.GetValue<string>("Seq:ServerUrl");

    var loggerConfiguration = new LoggerConfiguration()
      .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
      .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
      .Enrich.WithMachineName()
      .Enrich.WithEnvironmentName()
      .WriteTo.Seq(seqServerUrl!);
    
    if(builder.Environment.IsDevelopment())
    {
      loggerConfiguration.WriteTo.Console();
    }
    
    Log.Logger = loggerConfiguration.CreateLogger();
    builder.Host.UseSerilog();
    return builder;
  }
}