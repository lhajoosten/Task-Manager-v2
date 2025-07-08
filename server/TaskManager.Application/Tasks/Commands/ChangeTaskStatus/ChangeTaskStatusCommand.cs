using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Commands.ChangeTaskStatus;

public record ChangeTaskStatusCommand(
	Guid TaskId,
	TaskStatusType Status
) : IRequest<Result<TaskDto>>;
