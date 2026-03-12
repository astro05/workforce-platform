using WorkforceAPI.Domain.Events;

namespace WorkforceAPI.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher,
            RabbitMqPublisher>();

        return services;
    }
}