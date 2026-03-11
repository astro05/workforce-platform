namespace WorkforceAPI.Domain.Entities
{
    public class Designation : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Level { get; set; }

        // Navigation
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
