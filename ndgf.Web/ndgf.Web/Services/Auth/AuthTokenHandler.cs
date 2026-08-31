using System.Net;
using System.Net.Http.Headers;

namespace ndgf.Web.Services.Auth;

public class AuthTokenHandler(
  IHttpContextAccessor httpContextAccessor,
  IHttpClientFactory httpClientFactory,
  TokenStore tokenStore) : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
  {
    if (tokenStore.AccessToken is null)
    {
      tokenStore.AccessToken = httpContextAccessor.HttpContext?.User.FindFirst("AccessToken")?.Value;
      tokenStore.RefreshToken = httpContextAccessor.HttpContext?.User.FindFirst("RefreshToken")?.Value;
    }

    if (!string.IsNullOrWhiteSpace(tokenStore.AccessToken))
    {
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
    }

    var response = await base.SendAsync(request, cancellationToken);

    if (response.StatusCode == HttpStatusCode.Unauthorized)
    {
      var attemptedAccessToken = tokenStore.AccessToken;

      using (await tokenStore.LockAsync())
      {
        if (tokenStore.AccessToken == attemptedAccessToken && !string.IsNullOrWhiteSpace(tokenStore.RefreshToken))
        {
          var rawClient = httpClientFactory.CreateClient("RawApiClient");

          var refreshResponse = await rawClient.PostAsJsonAsync(
            "/api/users/refresh",
            new { RefreshToken = tokenStore.RefreshToken },
            cancellationToken);

          if (refreshResponse.IsSuccessStatusCode)
          {
            var result = await refreshResponse.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: cancellationToken);

            if (result is not null)
            {
              tokenStore.AccessToken = result.AccessToken;
              tokenStore.RefreshToken = result.RefreshToken;

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
            }
          }
        }
      }

      if (!string.IsNullOrWhiteSpace(tokenStore.AccessToken))
      {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
        response = await base.SendAsync(request, cancellationToken);
      }
    }

    return response;
  }
}