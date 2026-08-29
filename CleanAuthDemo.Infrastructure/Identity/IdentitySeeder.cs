using System.Security.Claims;
using CleanAuthDemo.Application.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CleanAuthDemo.Infrastructure.Identity;

public sealed class IdentitySeeder
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public IdentitySeeder(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await EnsureRoleAsync(
            Roles.Admin,
            [
                Permissions.Products.Read,
                Permissions.Products.Create,
                Permissions.Products.Update,
                Permissions.Products.Delete
            ]);

        await EnsureRoleAsync(
            Roles.Member,
            [
                Permissions.Products.Read
            ]);
    }

    private async Task EnsureRoleAsync(string roleName, IEnumerable<string> permissions)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            role = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName
            };

            var createResult = await _roleManager.CreateAsync(role);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(x => x.Description));

                throw new InvalidOperationException(errors);
            }
        }

        var existingClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var permission in permissions)
        {
            var exists = existingClaims.Any(x =>
                    x.Type == CustomClaimTypes.Permission &&
                    x.Value == permission);

            if (exists)
            {
                continue;
            }

            await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        }
    }
}