using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Application.Authorization;
using CleanAuthDemo.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAuthDemo.WebApi.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    private readonly ICurrentUser _currentUser;
    private readonly IAuthorizationService _authorizationService;
    public AccountController(ICurrentUser currentUser, IAuthorizationService authorizationService)
    {
        _currentUser = currentUser;
        _authorizationService = authorizationService;
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            _currentUser.UserId,
            _currentUser.Email,
            _currentUser.Permissions
        });
    }

    [Authorize]
    [HttpGet("owned-resource/{ownerUserId:guid}")]
    public async Task<IActionResult> GetOwnedResource(
    Guid ownerUserId)
    {
        var resource = new UserOwnedResource(ownerUserId);

        var authorizationResult = await _authorizationService.AuthorizeAsync(
                User,
                resource,
                AuthorizationPolicies.ResourceOwner);

        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        return Ok(new
        {
            Message = "You can access this resource.",
            OwnerUserId = ownerUserId
        });
    }
}