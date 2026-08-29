using System.Text;
using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.Infrastructure.Authentication;
using CleanAuthDemo.Infrastructure.Identity;
using CleanAuthDemo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAuthDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddIdentity(services);
        AddAuthenticationServices(services, configuration);

        services.AddScoped<IdentitySeeder>();

        return services;
    }

    private static void AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
    }

    private static void AddIdentity(
        IServiceCollection services)
    {
        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();
    }

    private static void AddAuthenticationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "JWT Issuer is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "JWT Audience is required.")
            .Validate(
                options =>
                    Encoding.UTF8.GetByteCount(options.SecretKey) >= 32,
                "JWT SecretKey must be at least 32 bytes.")
            .Validate(
                options => options.ExpirationMinutes > 0,
                "JWT ExpirationMinutes must be greater than zero.")
            .ValidateOnStart();

        services
            .AddOptions<RefreshTokenOptions>()
            .Bind(configuration.GetSection(
                RefreshTokenOptions.SectionName))
            .Validate(
                options => options.ExpirationDays > 0,
                "Refresh token expiration days must be greater than zero.")
            .ValidateOnStart();

        services.AddSingleton<
            IAccessTokenGenerator,
            JwtTokenGenerator>();

        services.AddScoped<
            IRefreshTokenService,
            RefreshTokenService>();
    }
}