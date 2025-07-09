// TaskManager.Domain/Events/TaskCreatedEvent.cs

// TaskManager.Domain/Events/TaskCreatedEvent.cs
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Tasks;

public class TaskCreatedEvent : IDomainEvent
{
	public TaskItem Task { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public TaskCreatedEvent(TaskItem task)
	{
		Task = task;
	}
}
