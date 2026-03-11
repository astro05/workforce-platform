using WorkforceAPI.Application.DTOs;

namespace WorkforceAPI.Application.Services;

public interface IDesignationService
{
    Task<IEnumerable<DesignationDto>> GetAllAsync(CancellationToken ct = default);
    Task<DesignationDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<DesignationDto> CreateAsync(CreateDesignationDto dto, CancellationToken ct = default);
    Task<DesignationDto> UpdateAsync(int id, UpdateDesignationDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}