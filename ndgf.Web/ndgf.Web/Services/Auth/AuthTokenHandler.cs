using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace ndgf.Web.Services.Auth;

public class AuthTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    var token = httpContextAccessor.HttpContext?.User.FindFirst("AccessToken")?.Value;

    if (!string.IsNullOrWhiteSpace(token))
    {
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    return await base.SendAsync(request, cancellationToken);
  }
}