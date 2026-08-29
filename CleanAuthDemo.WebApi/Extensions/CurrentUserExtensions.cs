using CleanAuthDemo.Application.Authentication;
using CleanAuthDemo.WebApi.Authentication;

namespace CleanAuthDemo.WebApi.Extensions;

public static class CurrentUserExtensions
{
    public static IServiceCollection AddCurrentUser(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<
            ICurrentUser,
            CurrentUser>();

        return services;
    }
}