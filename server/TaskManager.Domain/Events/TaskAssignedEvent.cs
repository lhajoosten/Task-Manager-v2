using TaskManager.Domain.Entities;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Events;

public class TaskAssignedEvent : IDomainEvent
{
	public TaskItem Task { get; }
	public UserId PreviousAssignedTo { get; }
	public UserId NewAssignedTo { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public TaskAssignedEvent(TaskItem task, UserId previousAssignedTo, UserId newAssignedTo)
	{
		Task = task;
		PreviousAssignedTo = previousAssignedTo;
		NewAssignedTo = newAssignedTo;
	}
}
