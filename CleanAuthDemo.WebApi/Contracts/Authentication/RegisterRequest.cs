namespace CleanAuthDemo.WebApi.Contracts.Authentication;

public sealed record RegisterRequest(
    string Email,
    string Password);