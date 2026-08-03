using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace ndgf.Web.Services.Auth;


public class AuthTokenHandler(IJSRuntime jsRuntime) : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    try
    {
      var token = await jsRuntime.InvokeAsync<string?>("ndgfAuth.getCookie", "accessToken");

      if (!string.IsNullOrWhiteSpace(token))
      {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      }
    }
    catch (InvalidOperationException)
    {
      // JS interop pas encore disponible (prerendering) : on continue sans token
    }

    return await base.SendAsync(request, cancellationToken);
  }
}