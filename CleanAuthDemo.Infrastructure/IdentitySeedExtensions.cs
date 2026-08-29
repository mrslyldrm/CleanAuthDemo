using CleanAuthDemo.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAuthDemo.Infrastructure;

public static class IdentitySeedExtensions
{
    public static async Task SeedIdentityAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

        await seeder.SeedAsync();
    }
}