using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Dtos;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated request is missing a user id claim."));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasks([FromQuery] TaskItemStatus? status)
    {
        var query = _db.Tasks.Where(t => t.UserId == CurrentUserId);

        if (status is not null)
        {
            query = query.Where(t => t.Status == status);
        }

        var tasks = await query
            .OrderBy(t => t.DueDate)
            .Select(t => ToResponse(t))
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetTaskById(Guid id)
    {
        var task = await FindOwnedTaskAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            DueDate = request.DueDate,
            UserId = CurrentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, ToResponse(task));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var task = await FindOwnedTaskAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(ToResponse(task));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await FindOwnedTaskAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private Task<TaskItem?> FindOwnedTaskAsync(Guid id) =>
        _db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == CurrentUserId);

    private static TaskResponse ToResponse(TaskItem task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status,
        task.DueDate,
        task.CreatedAt,
        task.UpdatedAt);
}
