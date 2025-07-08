using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Events;

public class TaskStatusChangedEvent : IDomainEvent
{
	public TaskItem Task { get; }
	public TaskStatusType PreviousStatus { get; }
	public TaskStatusType NewStatus { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public TaskStatusChangedEvent(TaskItem task, TaskStatusType previousStatus, TaskStatusType newStatus)
	{
		Task = task;
		PreviousStatus = previousStatus;
		NewStatus = newStatus;
	}
}
