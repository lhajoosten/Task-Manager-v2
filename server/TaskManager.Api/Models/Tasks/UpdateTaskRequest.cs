using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Models.Tasks;

public class UpdateTaskRequest
{
	[Required]
	[StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
	public string Title { get; set; } = string.Empty;

	[StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
	public string Description { get; set; } = string.Empty;

	public DateTime? DueDate { get; set; }

	public TaskPriorityType? Priority { get; set; }
}
