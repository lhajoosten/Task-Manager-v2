using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Tasks;

public class TaskDeletedEvent : IDomainEvent
{
    public TaskItem Task { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public TaskDeletedEvent(TaskItem task)
    {
        Task = task;
    }
}
