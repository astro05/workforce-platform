namespace WorkforceAPI.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

    public enum ProjectStatus
    {
        Active,
        OnHold,
        Completed,
        Cancelled
    }
}
