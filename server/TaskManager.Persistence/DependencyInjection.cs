using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Auth.Services;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Repositories;
using TaskManager.Persistence.Data;
using TaskManager.Persistence.Repositories;
using TaskManager.Persistence.Services;

namespace TaskManager.Persistence;

public static class DependencyInjection
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Database Context
		services.AddDbContext<TaskManagerDbContext>(options =>
			options.UseSqlServer(
				configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly(typeof(TaskManagerDbContext).Assembly.FullName)));

		// Register the context as IApplicationDbContext
		services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<TaskManagerDbContext>());

		// Repositories
		services.AddScoped<ITaskRepository, TaskRepository>();
		services.AddScoped<IUserRepository, UserRepository>();

		// Services
		services.AddScoped<ICurrentUserService, CurrentUserService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IPasswordService, PasswordService>();

		return services;
	}
}
