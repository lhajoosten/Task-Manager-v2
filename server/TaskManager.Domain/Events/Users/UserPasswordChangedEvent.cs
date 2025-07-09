using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;
public class UserPasswordChangedEvent : IDomainEvent
{
    public User User { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserPasswordChangedEvent(User user)
    {
        User = user;
    }
}

