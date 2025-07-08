// TaskManager.Domain/Events/TaskCreatedEvent.cs
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events;

public class TaskUpdatedEvent : IDomainEvent
{
	public TaskItem Task { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public TaskUpdatedEvent(TaskItem task)
	{
		Task = task;
	}
}