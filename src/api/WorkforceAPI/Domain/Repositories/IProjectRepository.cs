using WorkforceAPI.Domain.Entities;

namespace WorkforceAPI.Domain.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Project>> GetAllWithDetailsAsync(CancellationToken ct = default);
    Task AddMemberAsync(int projectId, int employeeId, string role = "Member", CancellationToken ct = default);
    Task RemoveMemberAsync(int projectId, int employeeId, CancellationToken ct = default);
}
