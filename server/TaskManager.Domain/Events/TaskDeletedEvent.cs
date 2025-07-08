using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events;

namespace TaskManager.Domain.Events;

public class TaskDeletedEvent : IDomainEvent
{
  public TaskItem Task { get; }
  public DateTime OccurredOn { get; } = DateTime.UtcNow;

  public TaskDeletedEvent(TaskItem task)
  {
    Task = task;
  }
}
