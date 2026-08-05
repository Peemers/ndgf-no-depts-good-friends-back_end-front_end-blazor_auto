using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ndgf.Api.Extensions;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
  public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
  {
    var securityScheme = new OpenApiSecurityScheme
    {
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer",
      BearerFormat = "JWT",
      In = ParameterLocation.Header,
      Name = "Authorization"
    };

    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
    document.Components.SecuritySchemes["Bearer"] = securityScheme;

    return Task.CompletedTask;
  }
}