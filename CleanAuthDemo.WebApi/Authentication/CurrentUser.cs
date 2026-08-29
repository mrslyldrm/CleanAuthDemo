using System.IdentityModel.Tokens.Jwt;
using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Application.Authorization;

namespace CleanAuthDemo.WebApi.Authentication;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? HttpContext =>
        _httpContextAccessor.HttpContext;

    public bool IsAuthenticated =>
        HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var value =
                HttpContext?.User.FindFirst(
                    JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }
    }

    public string? Email =>
        HttpContext?.User.FindFirst(
            JwtRegisteredClaimNames.Email)?.Value;

    public IReadOnlyCollection<string> Permissions =>
        HttpContext?.User
            .FindAll(CustomClaimTypes.Permission)
            .Select(x => x.Value)
            .ToArray()
        ?? [];
}