namespace WorkforceAPI.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? AvatarUrl { get; set; }
        public string[] Skills { get; set; } = [];

        // Foreign Keys
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public Designation Designation { get; set; } = null!;
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
