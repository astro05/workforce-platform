using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<(IEnumerable<Employee> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? search = null,
        int? departmentId = null,
        bool? isActive = null,
        CancellationToken ct = default);

    Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default);
}
