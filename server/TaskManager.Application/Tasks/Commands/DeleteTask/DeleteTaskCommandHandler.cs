using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
	private readonly ITaskRepository _taskRepository;
	private readonly IApplicationDbContext _context;
	private readonly ICurrentUserService _currentUserService;

	public DeleteTaskCommandHandler(
		ITaskRepository taskRepository,
		IApplicationDbContext context,
		ICurrentUserService currentUserService)
	{
		_taskRepository = taskRepository;
		_context = context;
		_currentUserService = currentUserService;
	}

	public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
	{
		var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
		if (task == null)
		{
			return Result.Failure("Task not found.");
		}

		var currentUserId = _currentUserService.GetCurrentUserId();
		if (currentUserId == null || !task.IsAssignedTo(currentUserId))
		{
			return Result.Failure("Unauthorized to delete this task.");
		}

		task.Delete();
		_taskRepository.Update(task);
		await _context.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}