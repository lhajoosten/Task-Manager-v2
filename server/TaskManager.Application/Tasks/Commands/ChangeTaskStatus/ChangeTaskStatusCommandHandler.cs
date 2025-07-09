using AutoMapper;

using MediatR;

using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ChangeTaskStatusCommandHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _context = context;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<TaskDto>> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            return Result.Failure<TaskDto>("Task not found.");
        }

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId is null || !task.IsAssignedTo(currentUserId))
        {
            return Result.Failure<TaskDto>("Unauthorized to update this task.");
        }

        switch (request.Status)
        {
            case TaskStatusType.Todo:
            task.MarkAsTodo();
            break;
            case TaskStatusType.InProgress:
            task.MarkAsInProgress();
            break;
            case TaskStatusType.Completed:
            task.MarkAsCompleted();
            break;
            case TaskStatusType.Cancelled:
            task.Cancel();
            break;
            default:
            return Result.Failure<TaskDto>("Invalid status transition.");
        }

        _taskRepository.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        var taskDto = _mapper.Map<TaskDto>(task);
        return Result.Success(taskDto);
    }
}
