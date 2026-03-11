namespace WorkforceAPI.Domain.Entities
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "Member";

        // Navigation
        public Project Project { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
