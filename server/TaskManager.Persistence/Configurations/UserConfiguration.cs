using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.Id)
			.ValueGeneratedNever();

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(u => u.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(u => u.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(500);

		builder.Property(u => u.IsActive)
			.HasDefaultValue(true);

		builder.Property(u => u.IsDeleted)
			.HasDefaultValue(false);

		builder.Property(u => u.CreatedAt)
			.IsRequired()
			.HasDefaultValueSql("GETUTCDATE()");

		builder.Property(u => u.UpdatedAt)
			.IsRequired(false);

		builder.Property(u => u.LastLoginAt)
			.IsRequired(false);

		// Indexes
		builder.HasIndex(u => u.Email)
			.IsUnique()
			.HasDatabaseName("IX_Users_Email");

		builder.HasIndex(u => new { u.FirstName, u.LastName })
			.HasDatabaseName("IX_Users_Name");

		builder.HasIndex(u => u.IsActive)
			.HasDatabaseName("IX_Users_IsActive");

		// Global query filter to exclude soft-deleted users
		builder.HasQueryFilter(u => !u.IsDeleted);

		// Ignore domain events property for EF Core
		builder.Ignore(u => u.DomainEvents);

		// Configure the backing field for Tasks collection
		builder.Metadata.FindNavigation(nameof(User.Tasks))!
			.SetPropertyAccessMode(PropertyAccessMode.Field);
	}
}
