namespace CleanAuthDemo.Application.Authentication;

public sealed record RefreshTokenRotationResult(
    RefreshTokenRotationStatus Status,
    Guid? UserId = null,
    string? RefreshToken = null,
    DateTime? ExpiresAtUtc = null);