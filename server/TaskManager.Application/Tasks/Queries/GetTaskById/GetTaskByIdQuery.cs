using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskDto>>;
