using MediatR;

using TaskManager.Application.Common.Models;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate = null,
    TaskPriorityType? Priority = null
) : IRequest<Result<TaskDto>>;
