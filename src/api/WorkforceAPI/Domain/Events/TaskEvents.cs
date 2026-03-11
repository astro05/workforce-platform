namespace WorkforceAPI.Domain.Events
{
    public record TaskCreatedEvent(
        int TaskId,
        string Title,
        int ProjectId,
        string Status
    ) : BaseDomainEvent
    {
        public override string EventType => "task.created";
    }

    public record TaskStatusChangedEvent(
        int TaskId,
        string Title,
        int ProjectId,
        string OldStatus,
        string NewStatus,
        int? AssignedToId
    ) : BaseDomainEvent
    {
        public override string EventType => "task.status_changed";
    }
}
