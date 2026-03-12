namespace WorkforceAPI.Domain.Events;

public interface IEventPublisher
{
    Task PublishAsync<T>(
        T domainEvent,
        CancellationToken ct = default)
        where T : BaseDomainEvent;
}