namespace CleanAuthDemo.Application.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(IIdentityService identityService, IAccessTokenGenerator accessTokenGenerator, IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _accessTokenGenerator = accessTokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public Task<Guid> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        return _identityService.CreateUserAsync(
            email,
            password,
            cancellationToken);
    }

    public async Task<AuthTokenResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _identityService.ValidateCredentialsAsync(email, password, cancellationToken);

        if (user is null)
        {
            return null;
        }

        AccessTokenResult accessToken = _accessTokenGenerator.Generate(user);
        RefreshTokenResult refreshToken = await _refreshTokenService.CreateAsync(user.Id, cancellationToken);

        return new AuthTokenResult(accessToken.AccessToken, accessToken.ExpiresAtUtc, refreshToken.RefreshToken, refreshToken.ExpiresAtUtc);
    }
    public async Task<AuthTokenResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        //Refresh token'ı döndürmek için _refreshTokenService.RotateAsync metodunu çağırıyoruz. Neden RotateAsync metodunu çağırıyoruz? Çünkü refresh token'lar tek kullanımlık olmalıdır. Yani bir refresh token kullanıldıktan sonra geçersiz hale gelmelidir. Bu nedenle, kullanıcı yeni bir access token almak istediğinde, eski refresh token'ı kullanarak yeni bir refresh token alması gerekir. RotateAsync metodu, bu işlemi gerçekleştirir ve yeni bir refresh token döndürür.
        var rotation = await _refreshTokenService.RotateAsync(refreshToken, cancellationToken);

        if (rotation.Status != RefreshTokenRotationStatus.Success ||
            rotation.UserId is null ||
            rotation.RefreshToken is null ||
            rotation.ExpiresAtUtc is null)
        {
            return null;
        }

        var user = await _identityService.GetUserAsync(rotation.UserId.Value, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var accessToken = _accessTokenGenerator.Generate(user);

        return new AuthTokenResult(accessToken.AccessToken, accessToken.ExpiresAtUtc, rotation.RefreshToken, rotation.ExpiresAtUtc.Value);
    }

    public Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return _refreshTokenService.RevokeSessionAsync(refreshToken, cancellationToken);
    }
}