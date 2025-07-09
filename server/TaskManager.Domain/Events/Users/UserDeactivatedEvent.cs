using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserDeactivatedEvent : IDomainEvent
{
	public User User { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public UserDeactivatedEvent(User user)
	{
		User = user;
	}
}
