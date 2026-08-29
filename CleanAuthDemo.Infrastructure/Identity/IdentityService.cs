using System.Security.Claims;
using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Application.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CleanAuthDemo.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Guid> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                Roles.Member);

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                ", ",
                roleResult.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        return user.Id;
    }

    public async Task<AuthenticatedUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await GetEffectivePermissionsAsync(user, roles);

        return new AuthenticatedUser(user.Id, user.Email!, roles.ToArray(), permissions);
    }

    public async Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await GetEffectivePermissionsAsync(user, roles);

        return new AuthenticatedUser(user.Id, user.Email!, roles.ToArray(), permissions);
    }

    private async Task<IReadOnlyCollection<string>> GetEffectivePermissionsAsync(ApplicationUser user, IList<string> roles)
    {
        var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Kullanıcıya doğrudan verilmiş permission'lar
        var userClaims = await _userManager.GetClaimsAsync(user);

        foreach (var claim in userClaims)
        {
            if (claim.Type == CustomClaimTypes.Permission)
            {
                permissions.Add(claim.Value);
            }
        }

        // Role üzerinden gelen permission'lar
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                continue;
            }

            var roleClaims =
                await _roleManager.GetClaimsAsync(role);

            foreach (var claim in roleClaims)
            {
                if (claim.Type == CustomClaimTypes.Permission)
                {
                    permissions.Add(claim.Value);
                }
            }
        }

        return permissions.ToArray();
    }
}