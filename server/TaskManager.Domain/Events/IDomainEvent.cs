using MediatR;

namespace TaskManager.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
