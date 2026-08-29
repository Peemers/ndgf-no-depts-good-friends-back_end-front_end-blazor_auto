namespace ndgf.Web.Endpoints;

public record RefreshCookieRequest(string AccessToken, string RefreshToken);