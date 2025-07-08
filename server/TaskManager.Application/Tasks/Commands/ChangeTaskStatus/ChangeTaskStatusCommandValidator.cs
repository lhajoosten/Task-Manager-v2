using FluentValidation;

namespace TaskManager.Application.Tasks.Commands.ChangeTaskStatus;

public class ChangeTaskStatusCommandValidator : AbstractValidator<ChangeTaskStatusCommand>
{
	public ChangeTaskStatusCommandValidator()
	{
		RuleFor(x => x.TaskId)
			.NotEmpty().WithMessage("Task ID is required.");

		RuleFor(x => x.Status)
			.IsInEnum().WithMessage("Invalid status value.");
	}
}
