using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Models;

namespace TaskManagementApi.Dtos;

public record TaskResponse(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateOnly DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title must be at most 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public TaskItemStatus Status { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    public DateOnly DueDate { get; set; }
}

public class UpdateTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title must be at most 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description must be at most 2000 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public TaskItemStatus Status { get; set; }

    [Required(ErrorMessage = "DueDate is required.")]
    public DateOnly DueDate { get; set; }
}
