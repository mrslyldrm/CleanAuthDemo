namespace CleanAuthDemo.Application.Authentication;

public interface IAuthService
{
    Task<Guid> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthTokenResult?> LoginAsync(
    string email,
    string password,
    CancellationToken cancellationToken = default);
}