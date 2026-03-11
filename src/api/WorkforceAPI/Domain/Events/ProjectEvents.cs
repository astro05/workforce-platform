namespace WorkforceAPI.Domain.Events
{
    public record ProjectCreatedEvent(
        int ProjectId,
        string Name,
        string Status
    ) : BaseDomainEvent
    {
        public override string EventType => "project.created";
    }

    public record ProjectUpdatedEvent(
        int ProjectId,
        string Name,
        string Status,
        object Before,
        object After
    ) : BaseDomainEvent
    {
        public override string EventType => "project.updated";
    }
}
