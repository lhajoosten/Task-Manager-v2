using Ardalis.GuardClauses;

using TaskManager.Domain.Common;
using TaskManager.Domain.Events;
using TaskManager.Domain.Events.Users;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }
    public bool IsDeleted { get; private set; }

    // Navigation property for tasks - use backing field to prevent direct access
    private readonly List<TaskItem> _tasks = [];
    public virtual IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    // For EF Core
    private User() { }

    public User(string email, string firstName, string lastName, string passwordHash)
    {
        Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
        FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
        PasswordHash = Guard.Against.NullOrWhiteSpace(passwordHash, nameof(passwordHash));

        ValidateEmail(email);

        IsActive = true;
        IsDeleted = false;

        AddDomainEvent(new UserRegisteredEvent(this));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        var hasChanges = false;

        if (!string.IsNullOrWhiteSpace(firstName) && FirstName != firstName)
        {
            FirstName = firstName;
            hasChanges = true;
        }

        if (!string.IsNullOrWhiteSpace(lastName) && LastName != lastName)
        {
            LastName = lastName;
            hasChanges = true;
        }

        if (hasChanges)
        {
            UpdateTimestamp();
            AddDomainEvent(new UserProfileUpdatedEvent(this));
        }
    }

    public void ChangePassword(string newPasswordHash)
    {
        Guard.Against.NullOrWhiteSpace(newPasswordHash, nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdateTimestamp();
        AddDomainEvent(new UserPasswordChangedEvent(this));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdateTimestamp();
        AddDomainEvent(new UserLoggedInEvent(this));
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            UpdateTimestamp();
            AddDomainEvent(new UserActivatedEvent(this));
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            UpdateTimestamp();
            AddDomainEvent(new UserDeactivatedEvent(this));
        }
    }

    public void Delete()
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            IsActive = false;
            UpdateTimestamp();
            AddDomainEvent(new UserDeletedEvent(this));
        }
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public UserId GetUserId() => UserId.From(Id);

    private static void ValidateEmail(string email)
    {
        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Domain Events
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
