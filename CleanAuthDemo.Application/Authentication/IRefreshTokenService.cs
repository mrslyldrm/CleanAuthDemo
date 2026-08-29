namespace CleanAuthDemo.Application.Authentication;

public interface IRefreshTokenService
{
    Task<RefreshTokenResult> CreateAsync(Guid userId, CancellationToken cancellationToken = default);
}