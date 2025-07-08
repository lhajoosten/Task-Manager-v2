// TaskManager.Domain/Events/TaskCreatedEvent.cs
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events;

public class TaskCreatedEvent : IDomainEvent
{
  public TaskItem Task { get; }
  public DateTime OccurredOn { get; } = DateTime.UtcNow;

  public TaskCreatedEvent(TaskItem task)
  {
    Task = task;
  }
}

// TaskManager.Domain/Events/TaskUpdatedEvent.cs
using TaskManager.Domain.Entities;

