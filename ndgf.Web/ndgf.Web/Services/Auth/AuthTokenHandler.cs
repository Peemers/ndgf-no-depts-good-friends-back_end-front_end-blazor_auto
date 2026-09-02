using System.Net;
using System.Net.Http.Headers;

namespace ndgf.Web.Services.Auth;

public class AuthTokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private const string AccessTokenKey = "CurrentAccessToken";
    private const string RefreshTokenKey = "CurrentRefreshToken";
    private const string RefreshTaskKey = "CurrentRefreshTask";
    private static readonly object LockObject = new();

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var accessToken = GetOrInitAccessToken(context);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshSucceeded = await GetOrStartRefresh(context, cancellationToken);

            if (refreshSucceeded)
            {
                var newAccessToken = context.Items[AccessTokenKey] as string;
                if (!string.IsNullOrWhiteSpace(newAccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
        }

        return response;
    }

    private string? GetOrInitAccessToken(HttpContext context)
    {
        if (context.Items.TryGetValue(AccessTokenKey, out var stored) && stored is string token)
        {
            return token;
        }

        var claimToken = context.User.FindFirst("AccessToken")?.Value;
        context.Items[AccessTokenKey] = claimToken;
        context.Items[RefreshTokenKey] = context.User.FindFirst("RefreshToken")?.Value;
        return claimToken;
    }

    private Task<bool> GetOrStartRefresh(HttpContext context, CancellationToken cancellationToken)
    {
        lock (LockObject)
        {
            if (context.Items[RefreshTaskKey] is Task<bool> existingTask && !existingTask.IsCompleted)
            {
                return existingTask;
            }

            var newTask = DoRefreshAsync(context, cancellationToken);
            context.Items[RefreshTaskKey] = newTask;
            return newTask;
        }
    }

    private async Task<bool> DoRefreshAsync(HttpContext context, CancellationToken cancellationToken)
    {
        var refreshToken = context.Items[RefreshTokenKey] as string;

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        var rawClient = httpClientFactory.CreateClient("RawApiClient");
        var refreshResponse = await rawClient.PostAsJsonAsync(
            "/api/users/refresh",
            new { RefreshToken = refreshToken },
            cancellationToken);

        if (!refreshResponse.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await refreshResponse.Content.ReadFromJsonAsync<RefreshResult>(cancellationToken: cancellationToken);
        if (result is null)
        {
            return false;
        }

        context.Items[AccessTokenKey] = result.AccessToken;
        context.Items[RefreshTokenKey] = result.RefreshToken;

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

        return true;
    }
}