namespace ndgf.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
  public async Task InvokeAsync(HttpContext httpContext)
  {
    try
    {
      await next(httpContext);
    }
    catch (Exception e)
    {
      logger.LogError(e, "Une Exception non gérée s'est produite");
      httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      httpContext.Response.ContentType = "application/json";
      await httpContext.Response.WriteAsJsonAsync(new { message = "Une erreur interne est survenue" });
    }
  }
}