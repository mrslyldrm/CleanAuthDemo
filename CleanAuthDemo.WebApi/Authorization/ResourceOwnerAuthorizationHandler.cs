using System.IdentityModel.Tokens.Jwt;
using CleanAuthDemo.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CleanAuthDemo.WebApi.Authorization;

public sealed class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, UserOwnedResource>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement, UserOwnedResource resource)
    {
        var userIdValue = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Task.CompletedTask;
        }

        if (resource.OwnerUserId == userId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}