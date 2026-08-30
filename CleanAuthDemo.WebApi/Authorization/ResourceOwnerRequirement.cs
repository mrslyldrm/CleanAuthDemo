using Microsoft.AspNetCore.Authorization;

namespace CleanAuthDemo.WebApi.Authorization;

public sealed class ResourceOwnerRequirement
    : IAuthorizationRequirement
{
}