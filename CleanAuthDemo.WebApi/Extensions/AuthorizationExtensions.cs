using CleanAuthDemo.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CleanAuthDemo.WebApi.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.ResourceOwner,
                policy =>
                {
                    policy.RequireAuthenticatedUser();

                    policy.AddRequirements(
                        new ResourceOwnerRequirement());
                });
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        services.AddScoped<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();

        return services;
    }
}