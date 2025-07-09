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

        // Configure foreign key relationship
        builder.Property(t => t.AssignedToId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        // Configure relationship with User
        builder.HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for better query performance
        builder.HasIndex(t => t.AssignedToId)
            .HasDatabaseName("IX_Tasks_AssignedToId");

        builder.HasIndex(t => new { t.Status, t.AssignedToId })
            .HasDatabaseName("IX_Tasks_Status_AssignedToId");

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
