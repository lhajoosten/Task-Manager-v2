using Ardalis.GuardClauses;
using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;
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
	public UserId AssignedTo { get; private set; } = null!;
	public bool IsDeleted { get; private set; }

	// For EF Core
	private TaskItem() { }

	public TaskItem(string title, string description, UserId assignedTo, DateTime? dueDate = null, TaskPriorityType priority = TaskPriorityType.Normal)
	{
		Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
		Description = Guard.Against.Null(description, nameof(description));
		AssignedTo = Guard.Against.Null(assignedTo, nameof(assignedTo));
		DueDate = dueDate;
		Priority = priority;
		Status = TaskStatusType.Todo;

		AddDomainEvent(new TaskCreatedEvent(this));
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

	public void AssignTo(UserId userId)
	{
		Guard.Against.Null(userId, nameof(userId));

		if (AssignedTo != userId)
		{
			var previousUserId = AssignedTo;
			AssignedTo = userId;
			UpdateTimestamp();
			AddDomainEvent(new TaskAssignedEvent(this, previousUserId, userId));
		}
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

	public bool IsAssignedTo(UserId userId)
	{
		return AssignedTo == userId;
	}

	// Domain Events
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}

	private void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}
}
