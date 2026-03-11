using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskService service,
        ILogger<TasksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/tasks?projectId={id} ──────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByProject(
        [FromQuery] int projectId,
        CancellationToken ct)
    {
        if (projectId <= 0)
            return BadRequest(new { message = "A valid projectId is required." });

        var tasks = await _service.GetByProjectAsync(projectId, ct);
        return Ok(tasks);
    }

    // ── GET /api/v1/tasks/{id} ────────────────────────────────
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var task = await _service.GetByIdAsync(id, ct);

        if (task is null)
        {
            _logger.LogWarning("Task {Id} not found", id);
            return NotFound(new { message = $"Task {id} not found." });
        }

        return Ok(task);
    }

    // ── POST /api/v1/tasks ────────────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto, ct);
        _logger.LogInformation(
            "Task created: {Id} - {Title} in Project {ProjectId}",
            created.Id, created.Title, created.ProjectId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    // ── PUT /api/v1/tasks/{id} ────────────────────────────────
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTaskDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            _logger.LogInformation(
                "Task updated: {Id} - {Title}", updated.Id, updated.Title);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── PATCH /api/v1/tasks/{id}/status ───────────────────────
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateTaskStatusDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Fetch current task, update only status
            var existing = await _service.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Task {id} not found.");

            var updateDto = new UpdateTaskDto
            {
                Title = existing.Title,
                Description = existing.Description,
                Status = dto.Status,
                Priority = Enum.Parse<Domain.Entities.TaskPriority>(existing.Priority),
                DueDate = existing.DueDate,
                AssignedToId = existing.AssignedToId
            };

            var updated = await _service.UpdateAsync(id, updateDto, ct);
            _logger.LogInformation(
                "Task {Id} status changed to {Status}", id, dto.Status);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── DELETE /api/v1/tasks/{id} ─────────────────────────────
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            _logger.LogInformation("Task deleted: {Id}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}