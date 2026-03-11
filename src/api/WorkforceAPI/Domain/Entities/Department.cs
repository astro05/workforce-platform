namespace WorkforceAPI.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Navigation
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
