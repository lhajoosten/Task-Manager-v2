using TaskManager.Domain.Entities;
using TaskManager.Persistence.Services;

namespace TaskManager.Persistence.Data;

public static class SeedData
{
    public static async Task SeedAsync(TaskManagerDbContext context)
    {
        // Check if we already have users
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        var passwordService = new PasswordService();

        // Create test users
        var users = new List<User>
        {
            new("admin@taskmanager.com", "Admin", "User", passwordService.HashPassword("Admin123!")),
            new("john.doe@example.com", "John", "Doe", passwordService.HashPassword("Password123!")),
            new("jane.smith@example.com", "Jane", "Smith", passwordService.HashPassword("Password123!"))
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Create sample tasks for the users (using the Guid directly)
        var tasks = new List<TaskItem>
        {
            new("Complete project documentation", "Write comprehensive documentation for the new feature", users[1].Id, DateTime.UtcNow.AddDays(7)),
            new("Review code changes", "Review and approve pending pull requests", users[1].Id, DateTime.UtcNow.AddDays(3)),
            new("Prepare presentation", "Create slides for the quarterly review meeting", users[2].Id, DateTime.UtcNow.AddDays(14)),
            new("Update website content", "Refresh the about page and team information", users[2].Id, DateTime.UtcNow.AddDays(5))
        };

        context.Tasks.AddRange(tasks);
        await context.SaveChangesAsync();
    }
}
