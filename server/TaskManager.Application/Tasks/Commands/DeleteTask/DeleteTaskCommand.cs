using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Result>;
