namespace CleanAuthDemo.Application.Authentication;

public sealed record AccessTokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);