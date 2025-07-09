using TaskManager.Domain.Events;

namespace TaskManager.Domain.Common;

public interface IAggregateRoot
{
	public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
	public void ClearDomainEvents();
	public void AddDomainEvent(IDomainEvent domainEvent);
}
