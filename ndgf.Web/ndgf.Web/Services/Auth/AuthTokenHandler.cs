using System.Net;
using System.Net.Http.Headers;

namespace ndgf.Web.Services.Auth;

public class AuthTokenHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory) : DelegatingHandler
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

    var response = await base.SendAsync(request, cancellationToken);

    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
      var refreshToken = httpContextAccessor.HttpContext?.User.FindFirst("RefreshToken")?.Value;

      if (!string.IsNullOrWhiteSpace(refreshToken))
      {
        var rawClient = httpClientFactory.CreateClient("RawApiClient");

        var refreshResponse = await rawClient.PostAsJsonAsync(
          "/api/users/refresh",
          new { RefreshToken = refreshToken },
          cancellationToken);

        if (refreshResponse.IsSuccessStatusCode)
        {
          var result = await refreshResponse.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: cancellationToken);

          if (result is not null)
          {
            var cookieClient = httpClientFactory.CreateClient("RawWebClient");

            var cookieHeader = httpContextAccessor.HttpContext?.Request.Headers.Cookie.ToString();
            if (!string.IsNullOrEmpty(cookieHeader))
            {
              cookieClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
            }

            await cookieClient.PostAsJsonAsync("/auth/refresh-cookie", new
            {
              result.AccessToken,
              result.RefreshToken
            }, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
          }
        }
      }
    }

    return response;
  }
}