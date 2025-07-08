using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
	string Title,
	string Description,
	DateTime? DueDate = null,
	TaskPriorityType Priority = TaskPriorityType.Normal
) : IRequest<Result<TaskDto>>;
