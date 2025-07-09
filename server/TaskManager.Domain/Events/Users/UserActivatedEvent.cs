using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserActivatedEvent : IDomainEvent
{
	public User User { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public UserActivatedEvent(User user)
	{
		User = user;
	}
}


