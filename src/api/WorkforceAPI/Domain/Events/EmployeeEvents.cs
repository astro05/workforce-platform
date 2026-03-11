namespace WorkforceAPI.Domain.Events
{
    public record EmployeeCreatedEvent(
        int EmployeeId,
        string FullName,
        string Email,
        string DepartmentName,
        string DesignationName
    ) : BaseDomainEvent
    {
        public override string EventType => "employee.created";
    }

    public record EmployeeUpdatedEvent(
        int EmployeeId,
        string FullName,
        string Email,
        object Before,
        object After
    ) : BaseDomainEvent
    {
        public override string EventType => "employee.updated";
    }

    public record EmployeeDeletedEvent(
        int EmployeeId,
        string FullName
    ) : BaseDomainEvent
    {
        public override string EventType => "employee.deleted";
    }
}
