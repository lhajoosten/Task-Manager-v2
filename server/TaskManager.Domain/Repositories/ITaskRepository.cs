using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Repositories;

public interface ITaskRepository
{
  Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IEnumerable<TaskItem>> FindAsync(ISpecification<TaskItem> specification, CancellationToken cancellationToken = default);
  Task<int> CountAsync(ISpecification<TaskItem> specification, CancellationToken cancellationToken = default);
  Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default);
  void Update(TaskItem task);
  void Delete(TaskItem task);
  Task<IEnumerable<TaskItem>> GetTasksForUserAsync(UserId userId, CancellationToken cancellationToken = default);
  Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(UserId? userId = null, CancellationToken cancellationToken = default);
}
