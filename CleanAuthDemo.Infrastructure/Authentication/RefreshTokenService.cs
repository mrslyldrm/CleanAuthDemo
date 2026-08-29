using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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
    public async Task<RefreshTokenRotationResult> RotateAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new RefreshTokenRotationResult(RefreshTokenRotationStatus.Invalid);
        }

        var tokenHash = HashToken(refreshToken);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var currentToken = await _dbContext.RefreshTokens.FromSqlInterpolated($"""
            SELECT * FROM "RefreshTokens" WHERE "TokenHash" = {tokenHash} 
            FOR UPDATE
            """)
            .SingleOrDefaultAsync(cancellationToken);

        if (currentToken is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new RefreshTokenRotationResult(RefreshTokenRotationStatus.Invalid);
        }

        var now = DateTime.UtcNow;

        if (currentToken.RevokedAtUtc is not null)
        {
            await RevokeFamilyAsync(currentToken.FamilyId, now, "Refresh token reuse detected.", cancellationToken);
            await transaction.RollbackAsync(cancellationToken);
            return new RefreshTokenRotationResult(RefreshTokenRotationStatus.Reused);
        }

        if (currentToken.ExpiresAtUtc <= now)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new RefreshTokenRotationResult(RefreshTokenRotationStatus.Expired);
        }

        var newRawToken = GenerateToken();
        var newTokenHash = HashToken(newRawToken);

        var newExpiresAtUtc = now.AddDays(_options.ExpirationDays);

        var newToken = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = currentToken.UserId,
            FamilyId = currentToken.FamilyId,
            TokenHash = newTokenHash,
            CreatedAtUtc = now,
            ExpiresAtUtc = newExpiresAtUtc
        };

        currentToken.RevokedAtUtc = now;
        currentToken.RevocationReason = "Rotated";
        currentToken.ReplacedByTokenId = newToken.Id;

        _dbContext.RefreshTokens.Add(newToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new RefreshTokenRotationResult(RefreshTokenRotationStatus.Success, currentToken.UserId, newRawToken, newExpiresAtUtc);
    }
    private async Task RevokeFamilyAsync(Guid familyId, DateTime revokedAtUtc, string reason, CancellationToken cancellationToken)
    {
        var activeTokens = await _dbContext.RefreshTokens.Where(x =>
                    x.FamilyId == familyId &&
                    x.RevokedAtUtc == null)
                .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAtUtc = revokedAtUtc;
            token.RevocationReason = reason;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }
        var tokenHash = HashToken(refreshToken);
        var currentToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (currentToken is null || currentToken.RevokedAtUtc is not null)
        {
            return;
        }
        var now = DateTime.UtcNow;
        await RevokeFamilyAsync(currentToken.FamilyId, now, "Session revoked by user.", cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        await _dbContext.RefreshTokens.Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.RevokedAtUtc, now).SetProperty(t => t.RevocationReason, "User logged out from all sessions."), cancellationToken);
    }
}