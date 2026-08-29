namespace CleanAuthDemo.Application.Authentication;

public interface IRefreshTokenService
{
    Task<RefreshTokenResult> CreateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RefreshTokenRotationResult> RotateAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default);
}