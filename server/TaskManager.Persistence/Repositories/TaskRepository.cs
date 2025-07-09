using Microsoft.EntityFrameworkCore;

using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.Specifications;
using TaskManager.Domain.ValueObjects;
using TaskManager.Persistence.Common;
using TaskManager.Persistence.Data;

namespace TaskManager.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagerDbContext _context;

    public TaskRepository(TaskManagerDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> FindAsync(ISpecification<TaskItem> specification, CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator<TaskItem>.GetQuery(_context.Tasks.AsQueryable(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TaskItem> specification, CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks.AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
        return task;
    }

    public void Update(TaskItem task)
    {
        _context.Tasks.Update(task);
    }

    public void Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksForUserAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var specification = new TasksByUserSpecification(userId);
        return await FindAsync(specification, cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync(UserId? userId = null, CancellationToken cancellationToken = default)
    {
        var specification = new OverdueTasksSpecification(userId);
        return await FindAsync(specification, cancellationToken);
    }
}