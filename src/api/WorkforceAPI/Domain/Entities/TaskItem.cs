namespace WorkforceAPI.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Backlog;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }

        // Foreign Keys
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }

        // Navigation
        public Project Project { get; set; } = null!;
        public Employee? AssignedTo { get; set; }
    }

    public enum TaskItemStatus
    {
        Backlog,
        Todo,
        InProgress,
        InReview,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}
