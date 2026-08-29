using System.Security.Cryptography;
using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CleanAuthDemo.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private const int TokenSizeInBytes = 32;

    private readonly ApplicationDbContext _dbContext;
    private readonly RefreshTokenOptions _options;

    public RefreshTokenService(ApplicationDbContext dbContext, IOptions<RefreshTokenOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task<RefreshTokenResult> CreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rawToken = GenerateToken();
        var tokenHash = HashToken(rawToken);

        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddDays(_options.ExpirationDays);

        var entity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FamilyId = Guid.NewGuid(),
            TokenHash = tokenHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = expiresAtUtc
        };

        _dbContext.RefreshTokens.Add(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(rawToken, expiresAtUtc);
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);

        return Base64UrlEncoder.Encode(bytes);
    }

    private static string HashToken(string token)
    {
        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hash);
    }
}