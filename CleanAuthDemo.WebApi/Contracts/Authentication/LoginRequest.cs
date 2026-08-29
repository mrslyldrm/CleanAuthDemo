namespace CleanAuthDemo.WebApi.Contracts.Authentication;

public sealed record LoginRequest(
    string Email,
    string Password);