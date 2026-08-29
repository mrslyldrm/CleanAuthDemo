using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAuthDemo.WebApi.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        var permissions = User.FindAll("permission").Select(x => x.Value).ToArray();

        return Ok(new
        {
            UserId = userId,
            Email = email,
            Permissions = permissions
        });
    }
}