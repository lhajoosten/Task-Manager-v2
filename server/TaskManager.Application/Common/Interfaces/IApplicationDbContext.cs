using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<TaskItem> Tasks { get; }
	DbSet<User> Users { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
