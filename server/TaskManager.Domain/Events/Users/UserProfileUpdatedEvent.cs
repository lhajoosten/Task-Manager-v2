using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserProfileUpdatedEvent : IDomainEvent
{
    public User User { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserProfileUpdatedEvent(User user)
    {
        User = user;
    }
}
