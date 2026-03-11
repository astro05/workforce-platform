using Microsoft.AspNetCore.Mvc;
using WorkforceAPI.Application.DTOs;
using WorkforceAPI.Application.Services;

namespace WorkforceAPI.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _service;
    private readonly ILogger<LeaveRequestsController> _logger;

    public LeaveRequestsController(
        ILeaveRequestService service,
        ILogger<LeaveRequestsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ── GET /api/v1/leaverequests ─────────────────────────────
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] LeaveQueryParams query,
        CancellationToken ct)
    {
        var requests = await _service.GetAllAsync(query, ct);
        return Ok(requests);
    }

    // ── GET /api/v1/leaverequests/employee/{employeeId} ───────
    [HttpGet("employee/{employeeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(
        int employeeId,
        CancellationToken ct)
    {
        var requests = await _service.GetByEmployeeAsync(employeeId, ct);
        return Ok(requests);
    }

    // ── GET /api/v1/leaverequests/{id} ────────────────────────
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken ct)
    {
        var request = await _service.GetByIdAsync(id, ct);

        if (request is null)
        {
            _logger.LogWarning("Leave request {Id} not found", id);
            return NotFound(new { message = $"Leave request {id} not found." });
        }

        return Ok(request);
    }

    // ── POST /api/v1/leaverequests ────────────────────────────
    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLeaveRequestDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var created = await _service.CreateAsync(dto, ct);
            _logger.LogInformation(
                "Leave request created: {Id} for Employee {EmployeeId}",
                created.Id, created.EmployeeId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── PUT /api/v1/leaverequests/{id}/status ─────────────────
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(LeaveRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        string id,
        [FromBody] UpdateLeaveStatusDto dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _service.UpdateStatusAsync(id, dto, ct);
            _logger.LogInformation(
                "Leave request {Id} status changed to {Status}",
                id, dto.Status);

            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // ── PUT /api/v1/leaverequests/{id}/cancel ─────────────────
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        string id,
        [FromQuery] string actorName,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(actorName))
            return BadRequest(new { message = "actorName is required." });

        try
        {
            await _service.CancelAsync(id, actorName, ct);
            _logger.LogInformation(
                "Leave request {Id} cancelled by {ActorName}",
                id, actorName);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}