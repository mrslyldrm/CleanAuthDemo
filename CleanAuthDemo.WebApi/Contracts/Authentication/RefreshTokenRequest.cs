namespace CleanAuthDemo.WebApi.Contracts.Authentication;

public sealed record RefreshTokenRequest(
    string RefreshToken);