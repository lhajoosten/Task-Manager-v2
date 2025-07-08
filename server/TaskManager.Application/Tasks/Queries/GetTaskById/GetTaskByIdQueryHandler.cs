using AutoMapper;
using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
	private readonly ITaskRepository _taskRepository;
	private readonly ICurrentUserService _currentUserService;
	private readonly IMapper _mapper;

	public GetTaskByIdQueryHandler(
		ITaskRepository taskRepository,
		ICurrentUserService currentUserService,
		IMapper mapper)
	{
		_taskRepository = taskRepository;
		_currentUserService = currentUserService;
		_mapper = mapper;
	}

	public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
	{
		var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
		if (task == null)
		{
			return Result.Failure<TaskDto>("Task not found.");
		}

		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == null || !task.IsAssignedTo(currentUserId))
		{
			return Result.Failure<TaskDto>("Unauthorized to view this task.");
		}

		var taskDto = _mapper.Map<TaskDto>(task);
		return Result.Success(taskDto);
	}
}
