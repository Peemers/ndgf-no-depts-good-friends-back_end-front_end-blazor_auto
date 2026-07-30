namespace ndgf.Application.Models.User;

public record LoginResult(Domain.Entities.User User, string AccessToken, string RefreshToken);