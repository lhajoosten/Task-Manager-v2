using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserRegisteredEvent : IDomainEvent
{
    public User User { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserRegisteredEvent(User user)
    {
        User = user;
    }
}
