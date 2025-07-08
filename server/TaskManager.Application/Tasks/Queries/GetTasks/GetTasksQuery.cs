using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Queries.GetTasks;

public record GetTasksQuery(
	int Page = 1,
	int PageSize = 10,
	TaskStatusType? Status = null,
	TaskPriorityType? Priority = null,
	string? Search = null,
	bool OnlyOverdue = false
) : IRequest<Result<PagedResult<TaskDto>>>;
