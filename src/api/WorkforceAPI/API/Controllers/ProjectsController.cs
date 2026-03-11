using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectService service,
        ILogger<ProjectsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/projects ──────────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var projects = await _service.GetAllAsync(ct);
        return Ok(projects);
    }

    // ── GET /api/v1/projects/{id} ─────────────────────────────
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var project = await _service.GetByIdAsync(id, ct);

        if (project is null)
        {
            _logger.LogWarning("Project {Id} not found", id);
            return NotFound(new { message = $"Project {id} not found." });
        }

        return Ok(project);
    }

    // ── POST /api/v1/projects ─────────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validate end date is after start date
        if (dto.EndDate.HasValue && dto.EndDate.Value <= dto.StartDate)
            return BadRequest(new { message = "EndDate must be after StartDate." });

        var created = await _service.CreateAsync(dto, ct);
        _logger.LogInformation(
            "Project created: {Id} - {Name}", created.Id, created.Name);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // ── PUT /api/v1/projects/{id} ─────────────────────────────
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProjectDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.EndDate.HasValue && dto.EndDate.Value <= dto.StartDate)
            return BadRequest(new { message = "EndDate must be after StartDate." });

        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            _logger.LogInformation(
                "Project updated: {Id} - {Name}", updated.Id, updated.Name);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── POST /api/v1/projects/{id}/members ────────────────────
    [HttpPost("{id:int}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(
        int id,
        [FromBody] AddProjectMemberDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _service.AddMemberAsync(id, dto.EmployeeId, dto.Role, ct);
            _logger.LogInformation(
                "Employee {EmployeeId} added to Project {ProjectId}",
                dto.EmployeeId, id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── DELETE /api/v1/projects/{id}/members/{employeeId} ─────
    [HttpDelete("{id:int}/members/{employeeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(
        int id,
        int employeeId,
        CancellationToken ct)
    {
        try
        {
            await _service.RemoveMemberAsync(id, employeeId, ct);
            _logger.LogInformation(
                "Employee {EmployeeId} removed from Project {ProjectId}",
                employeeId, id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}