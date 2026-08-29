namespace CleanAuthDemo.Application.Authentication;

public sealed record AuthTokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);