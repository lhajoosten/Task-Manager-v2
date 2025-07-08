using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Specifications;

public class TasksByUserSpecification : BaseSpecification<TaskItem>
{
  public TasksByUserSpecification(UserId userId)
      : base(task => task.AssignedTo == userId && !task.IsDeleted)
  {
    ApplyOrderByDescending(task => task.CreatedAt);
  }
}

public class TasksByStatusSpecification : BaseSpecification<TaskItem>
{
  public TasksByStatusSpecification(TaskStatus status, UserId? userId = null)
      : base(task => task.Status == status && !task.IsDeleted && (userId == null || task.AssignedTo == userId))
  {
    ApplyOrderByDescending(task => task.CreatedAt);
  }
}

public class OverdueTasksSpecification : BaseSpecification<TaskItem>
{
  public OverdueTasksSpecification(UserId? userId = null)
      : base(task => task.DueDate.HasValue &&
                    task.DueDate.Value.Date < DateTime.UtcNow.Date &&
                    task.Status != TaskStatus.Completed &&
                    !task.IsDeleted &&
                    (userId == null || task.AssignedTo == userId))
  {
    ApplyOrderBy(task => task.DueDate!);
  }
}

public class TasksByPrioritySpecification : BaseSpecification<TaskItem>
{
  public TasksByPrioritySpecification(TaskPriorityType priority, UserId? userId = null)
      : base(task => task.Priority == priority && !task.IsDeleted && (userId == null || task.AssignedTo == userId))
  {
    ApplyOrderByDescending(task => task.CreatedAt);
  }
}

public class TaskSearchSpecification : BaseSpecification<TaskItem>
{
  public TaskSearchSpecification(string searchTerm, UserId? userId = null)
      : base(task => (task.Title.Contains(searchTerm) || task.Description.Contains(searchTerm)) &&
                    !task.IsDeleted &&
                    (userId == null || task.AssignedTo == userId))
  {
    ApplyOrderByDescending(task => task.CreatedAt);
  }
}

public class PaginatedTasksSpecification : BaseSpecification<TaskItem>
{
  public PaginatedTasksSpecification(int skip, int take, UserId? userId = null, TaskStatus? status = null)
      : base(task => !task.IsDeleted &&
                    (userId == null || task.AssignedTo == userId) &&
                    (status == null || task.Status == status))
  {
    ApplyOrderByDescending(task => task.CreatedAt);
    ApplyPaging(skip, take);
  }
}
