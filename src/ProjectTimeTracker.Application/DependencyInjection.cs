using Microsoft.Extensions.DependencyInjection;
using ProjectTimeTracker.Application.Abstractions.Messaging;

namespace ProjectTimeTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator();
        services.AddMediatorHandlers(typeof(DependencyInjection).Assembly);

        return services;
    }
}