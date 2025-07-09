using Ardalis.GuardClauses;
using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;
using TaskManager.Domain.Events.Tasks;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Entities;

public class TaskItem : BaseEntity, IAggregateRoot
{
	private readonly List<IDomainEvent> _domainEvents = [];

	public string Title { get; private set; } = string.Empty;
	public string Description { get; private set; } = string.Empty;
	public DateTime? DueDate { get; private set; }
	public TaskStatusType Status { get; private set; } = TaskStatusType.Todo;
	public TaskPriorityType Priority { get; private set; } = TaskPriorityType.Normal;

	// Foreign key to User
	public Guid AssignedToId { get; private set; }

	// Navigation property
	public virtual User AssignedTo { get; private set; } = null!;

	public bool IsDeleted { get; private set; }

	// For EF Core
	private TaskItem() { }

	public TaskItem(string title, string description, Guid assignedToId, DateTime? dueDate = null, TaskPriorityType priority = TaskPriorityType.Normal)
	{
		Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
		Description = Guard.Against.Null(description, nameof(description));
		AssignedToId = Guard.Against.Default(assignedToId, nameof(assignedToId));
		DueDate = dueDate;
		Priority = priority;
		Status = TaskStatusType.Todo;

		AddDomainEvent(new TaskCreatedEvent(this));
	}

	// Overload that accepts UserId for backward compatibility
	public TaskItem(string title, string description, UserId assignedTo, DateTime? dueDate = null, TaskPriorityType priority = TaskPriorityType.Normal)
		: this(title, description, assignedTo.Value, dueDate, priority)
	{
	}

	public void UpdateDetails(string title, string description, DateTime? dueDate = null, TaskPriorityType? priority = null)
	{
		var hasChanges = false;

		if (!string.IsNullOrWhiteSpace(title) && Title != title)
		{
			Title = title;
			hasChanges = true;
		}

		if (description != Description)
		{
			Description = description ?? string.Empty;
			hasChanges = true;
		}

		if (dueDate != DueDate)
		{
			DueDate = dueDate;
			hasChanges = true;
		}

		if (priority.HasValue && Priority != priority.Value)
		{
			Priority = priority.Value;
			hasChanges = true;
		}

		if (hasChanges)
		{
			UpdateTimestamp();
			AddDomainEvent(new TaskUpdatedEvent(this));
		}
	}

	public void MarkAsInProgress()
	{
		if (Status == TaskStatusType.Todo)
		{
			Status = TaskStatusType.InProgress;
			UpdateTimestamp();
			AddDomainEvent(new TaskStatusChangedEvent(this, TaskStatusType.Todo, TaskStatusType.InProgress));
		}
	}

	public void MarkAsCompleted()
	{
		if (Status != TaskStatusType.Completed && Status != TaskStatusType.Cancelled)
		{
			var previousStatus = Status;
			Status = TaskStatusType.Completed;
			UpdateTimestamp();
			AddDomainEvent(new TaskStatusChangedEvent(this, previousStatus, TaskStatusType.Completed));
		}
	}

	public void MarkAsTodo()
	{
		if (Status == TaskStatusType.InProgress)
		{
			Status = TaskStatusType.Todo;
			UpdateTimestamp();
			AddDomainEvent(new TaskStatusChangedEvent(this, TaskStatusType.InProgress, TaskStatusType.Todo));
		}
	}

	public void Cancel()
	{
		if (Status != TaskStatusType.Completed && Status != TaskStatusType.Cancelled)
		{
			var previousStatus = Status;
			Status = TaskStatusType.Cancelled;
			UpdateTimestamp();
			AddDomainEvent(new TaskStatusChangedEvent(this, previousStatus, TaskStatusType.Cancelled));
		}
	}

	public void AssignTo(Guid userId)
	{
		Guard.Against.Default(userId, nameof(userId));

		if (AssignedToId != userId)
		{
			var previousUserId = AssignedToId;
			AssignedToId = userId;
			UpdateTimestamp();
			AddDomainEvent(new TaskAssignedEvent(this, UserId.From(previousUserId), UserId.From(userId)));
		}
	}

	// Overload that accepts UserId for backward compatibility
	public void AssignTo(UserId userId)
	{
		AssignTo(userId.Value);
	}

	public void Delete()
	{
		if (!IsDeleted)
		{
			IsDeleted = true;
			UpdateTimestamp();
			AddDomainEvent(new TaskDeletedEvent(this));
		}
	}

	public bool IsOverdue()
	{
		return DueDate.HasValue && DueDate.Value.Date < DateTime.UtcNow.Date && Status != TaskStatusType.Completed;
	}

	public bool IsAssignedTo(Guid userId)
	{
		return AssignedToId == userId;
	}

	// Overload that accepts UserId for backward compatibility
	public bool IsAssignedTo(UserId userId)
	{
		return AssignedToId == userId.Value;
	}

	// Helper method to get UserId for backward compatibility
	public UserId GetAssignedUserId()
	{
		return UserId.From(AssignedToId);
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
