using CleanAuthDemo.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CleanAuthDemo.WebApi.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}