namespace WorkforceAPI.Domain.Events
{
    public record LeaveRequestCreatedEvent(
        string LeaveRequestId,
        int EmployeeId,
        string EmployeeName,
        string LeaveType,
        DateTime StartDate,
        DateTime EndDate
    ) : BaseDomainEvent
    {
        public override string EventType => "leave.created";
    }

    public record LeaveRequestStatusChangedEvent(
        string LeaveRequestId,
        int EmployeeId,
        string EmployeeName,
        string OldStatus,
        string NewStatus,
        string? Comment
    ) : BaseDomainEvent
    {
        public override string EventType => "leave.status_changed";
    }
}
