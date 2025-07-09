using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public TaskStatusType Status { get; set; }
    public TaskPriorityType Priority { get; set; }
    public Guid AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsOverdue { get; set; }
}
