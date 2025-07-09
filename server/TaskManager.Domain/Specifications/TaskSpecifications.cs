using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Specifications;

public class TasksByUserSpecification : BaseSpecification<TaskItem>
{
	public TasksByUserSpecification(UserId userId)
		: base(task => task.AssignedToId == userId.Value && !task.IsDeleted)
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}

	public TasksByUserSpecification(Guid userId)
		: base(task => task.AssignedToId == userId && !task.IsDeleted)
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}
}

public class TasksByStatusSpecification : BaseSpecification<TaskItem>
{
	public TasksByStatusSpecification(TaskStatusType status, UserId? userId = null)
		: base(task => task.Status == status && !task.IsDeleted && (userId == null || task.AssignedToId == userId.Value))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}

	public TasksByStatusSpecification(TaskStatusType status, Guid? userId = null)
		: base(task => task.Status == status && !task.IsDeleted && (userId == null || task.AssignedToId == userId))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}
}

public class OverdueTasksSpecification : BaseSpecification<TaskItem>
{
	public OverdueTasksSpecification(UserId? userId = null)
		: base(task => task.DueDate.HasValue &&
					  task.DueDate.Value.Date < DateTime.UtcNow.Date &&
					  task.Status != TaskStatusType.Completed &&
					  !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId.Value))
	{
		ApplyOrderBy(task => task.DueDate!);
	}

	public OverdueTasksSpecification(Guid? userId = null)
		: base(task => task.DueDate.HasValue &&
					  task.DueDate.Value.Date < DateTime.UtcNow.Date &&
					  task.Status != TaskStatusType.Completed &&
					  !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId))
	{
		ApplyOrderBy(task => task.DueDate!);
	}
}

public class TasksByPrioritySpecification : BaseSpecification<TaskItem>
{
	public TasksByPrioritySpecification(TaskPriorityType priority, UserId? userId = null)
		: base(task => task.Priority == priority && !task.IsDeleted && (userId == null || task.AssignedToId == userId.Value))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}

	public TasksByPrioritySpecification(TaskPriorityType priority, Guid? userId = null)
		: base(task => task.Priority == priority && !task.IsDeleted && (userId == null || task.AssignedToId == userId))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}
}

public class TaskSearchSpecification : BaseSpecification<TaskItem>
{
	public TaskSearchSpecification(string searchTerm, UserId? userId = null)
		: base(task => (task.Title.Contains(searchTerm) || task.Description.Contains(searchTerm)) &&
					  !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId.Value))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}

	public TaskSearchSpecification(string searchTerm, Guid? userId = null)
		: base(task => (task.Title.Contains(searchTerm) || task.Description.Contains(searchTerm)) &&
					  !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
	}
}

public class PaginatedTasksSpecification : BaseSpecification<TaskItem>
{
	public PaginatedTasksSpecification(int skip, int take, UserId? userId = null, TaskStatusType? status = null)
		: base(task => !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId.Value) &&
					  (status == null || task.Status == status))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
		ApplyPaging(skip, take);
	}

	public PaginatedTasksSpecification(int skip, int take, Guid? userId = null, TaskStatusType? status = null)
		: base(task => !task.IsDeleted &&
					  (userId == null || task.AssignedToId == userId) &&
					  (status == null || task.Status == status))
	{
		ApplyOrderByDescending(task => task.CreatedAt);
		ApplyPaging(skip, take);
	}
}
