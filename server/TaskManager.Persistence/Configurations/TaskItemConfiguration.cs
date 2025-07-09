using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty);

        builder.Property(t => t.DueDate)
            .IsRequired(false);

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .HasDefaultValue(TaskStatusType.Todo);

        builder.Property(t => t.Priority)
            .HasConversion<int>()
            .HasDefaultValue(TaskPriorityType.Normal);

        builder.Property(t => t.AssignedTo)
            .HasConversion(
                userId => userId.Value,
                value => new(value))
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        // Indexes for better query performance
        builder.HasIndex(t => t.AssignedTo)
            .HasDatabaseName("IX_Tasks_AssignedTo");

        builder.HasIndex(t => new { t.Status, t.AssignedTo })
            .HasDatabaseName("IX_Tasks_Status_AssignedTo");

        builder.HasIndex(t => new { t.DueDate, t.Status })
            .HasDatabaseName("IX_Tasks_DueDate_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");

        // Global query filter to exclude soft-deleted items
        builder.HasQueryFilter(t => !t.IsDeleted);

        // Ignore domain events property for EF Core
        builder.Ignore(t => t.DomainEvents);
    }
}
