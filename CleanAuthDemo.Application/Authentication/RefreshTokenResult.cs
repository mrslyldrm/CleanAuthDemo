namespace CleanAuthDemo.Application.Authentication;

public sealed record RefreshTokenResult(
    string RefreshToken,
    DateTime ExpiresAtUtc);