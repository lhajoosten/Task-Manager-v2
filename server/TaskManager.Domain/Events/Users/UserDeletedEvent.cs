using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserDeletedEvent : IDomainEvent
{
	public User User { get; }
	public DateTime OccurredOn { get; } = DateTime.UtcNow;

	public UserDeletedEvent(User user)
	{
		User = user;
	}
}
