using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Persistence.Data;

public class TaskManagerDbContext : DbContext, IApplicationDbContext
{
	private readonly IMediator? _mediator;

	public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
		: base(options)
	{
	}

	public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options, IMediator mediator)
		: base(options)
	{
		_mediator = mediator;
	}

	public DbSet<TaskItem> Tasks => Set<TaskItem>();
	public DbSet<User> Users => Set<User>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		// Update timestamps before saving
		UpdateTimestamps();

		// Dispatch domain events
		if (_mediator != null)
		{
			await DispatchDomainEvents();
		}

		return await base.SaveChangesAsync(cancellationToken);
	}

	private void UpdateTimestamps()
	{
		var entries = ChangeTracker.Entries<BaseEntity>()
			.Where(e => e.State == EntityState.Modified);

		foreach (var entry in entries)
		{
			entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
		}
	}

	private async Task DispatchDomainEvents()
	{
		var aggregateRoots = ChangeTracker.Entries<IAggregateRoot>()
			.Where(x => x.Entity.DomainEvents.Count != 0)
			.Select(x => x.Entity)
			.ToList();

		var domainEvents = aggregateRoots
			.SelectMany(x => x.DomainEvents)
			.ToList();

		aggregateRoots.ForEach(x => x.ClearDomainEvents());

		if (_mediator != null)
		{
			foreach (var domainEvent in domainEvents)
			{
				await _mediator.Publish(domainEvent);
			}
		}
	}
}
