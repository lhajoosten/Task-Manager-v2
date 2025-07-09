using AutoMapper;

using MediatR;

using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(
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

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task == null)
        {
            return Result.Failure<TaskDto>("Task not found.");
        }

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId is null || !task.IsAssignedTo(currentUserId))
        {
            return Result.Failure<TaskDto>("Unauthorized to update this task.");
        }

        task.UpdateDetails(request.Title, request.Description, request.DueDate, request.Priority);
        _taskRepository.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        var taskDto = _mapper.Map<TaskDto>(task);
        return Result.Success(taskDto);
    }
}
