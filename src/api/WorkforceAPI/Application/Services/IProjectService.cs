using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProjectDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken ct = default);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken ct = default);
    Task AddMemberAsync(int projectId, int employeeId, string role, CancellationToken ct = default);
    Task RemoveMemberAsync(int projectId, int employeeId, CancellationToken ct = default);
}