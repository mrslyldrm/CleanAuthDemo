using CleanAuthDemo.Application.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAuthDemo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}