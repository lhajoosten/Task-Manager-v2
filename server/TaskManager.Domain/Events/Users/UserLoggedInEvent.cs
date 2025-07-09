using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events.Users;

public class UserLoggedInEvent : IDomainEvent
{
    public User User { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoggedInEvent(User user)
    {
        User = user;
    }
}


