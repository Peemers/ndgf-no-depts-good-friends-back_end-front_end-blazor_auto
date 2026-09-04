using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;

namespace ndgf.Web.Services.Auth;

public class AuthTokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private static readonly ConcurrentDictionary<string, RefreshResult> LatestTokens = new();
    private static readonly ConcurrentDictionary<string, Task<RefreshResult?>> RefreshTasks = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var cookieAccessToken = context.User.FindFirst("AccessToken")?.Value;
        var cookieRefreshToken = context.User.FindFirst("RefreshToken")?.Value;

        var accessToken = (userId is not null && LatestTokens.TryGetValue(userId, out var cached))
            ? cached.AccessToken
            : cookieAccessToken;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && userId is not null)
        {
            var refreshTokenToUse = LatestTokens.TryGetValue(userId, out var latest)
                ? latest.RefreshToken
                : cookieRefreshToken;

            if (!string.IsNullOrWhiteSpace(refreshTokenToUse))
            {
                var refreshTask = RefreshTasks.GetOrAdd(userId, _ => DoRefreshAsync(refreshTokenToUse, context, userId, cancellationToken));

                var result = await refreshTask;

                RefreshTasks.TryRemove(new KeyValuePair<string, Task<RefreshResult?>>(userId, refreshTask));

                if (result is not null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
        }

        return response;
    }

    private async Task<RefreshResult?> DoRefreshAsync(string refreshToken, HttpContext context, string userId, CancellationToken cancellationToken)
    {
        var rawClient = httpClientFactory.CreateClient("RawApiClient");
        var refreshResponse = await rawClient.PostAsJsonAsync(
            "/api/users/refresh",
            new { RefreshToken = refreshToken },
            cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var result = await refreshResponse.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: cancellationToken);
        if (result is null)
        {
            return null;
        }

        LatestTokens[userId] = result;

        var cookieClient = httpClientFactory.CreateClient("RawWebClient");
        var cookieHeader = context.Request.Headers.Cookie.ToString();
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            cookieClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
        }

        await cookieClient.PostAsJsonAsync("/auth/refresh-cookie", new
        {
            result.AccessToken,
            result.RefreshToken
        }, cancellationToken);

        return result;
    }
}