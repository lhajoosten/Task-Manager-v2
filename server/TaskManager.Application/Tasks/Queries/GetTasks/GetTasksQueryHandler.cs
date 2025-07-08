using AutoMapper;
using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;
using TaskManager.Domain.Specifications;

namespace TaskManager.Application.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<PagedResult<TaskDto>>>
{
	private readonly ITaskRepository _taskRepository;
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper _mapper;

	public GetTasksQueryHandler(
		ITaskRepository taskRepository,
		ICurrentUserService currentUserService,
		IMapper mapper)
	{
		_taskRepository = taskRepository;
		_currentUserService = currentUserService;
		_mapper = mapper;
	}

	public async Task<Result<PagedResult<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
	{
		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == null)
		{
			return Result.Failure<PagedResult<TaskDto>>("User not authenticated.");
		}

		var skip = (request.Page - 1) * request.PageSize;

		// Change this line - use the interface type instead of concrete type
		ISpecification<TaskItem> specification = new PaginatedTasksSpecification(skip, request.PageSize, currentUserId, request.Status);

		// Apply additional filters if needed
		if (request.OnlyOverdue)
		{
			specification = new OverdueTasksSpecification(currentUserId);
		}
		else if (!string.IsNullOrWhiteSpace(request.Search))
		{
			specification = new TaskSearchSpecification(request.Search, currentUserId);
		}
		else if (request.Priority.HasValue)
		{
			specification = new TasksByPrioritySpecification(request.Priority.Value, currentUserId);
		}

		var tasks = await _taskRepository.FindAsync(specification, cancellationToken);
		var totalCount = await _taskRepository.CountAsync(new TasksByUserSpecification(currentUserId), cancellationToken);

		var taskDtos = _mapper.Map<IEnumerable<TaskDto>>(tasks);
		var pagedResult = new PagedResult<TaskDto>(taskDtos, totalCount, request.Page, request.PageSize);

		return Result.Success(pagedResult);
	}
}
