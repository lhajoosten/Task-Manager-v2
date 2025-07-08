using FluentValidation;

namespace TaskManager.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
	public UpdateTaskCommandValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Task ID is required.");

		RuleFor(x => x.Title)
			.NotEmpty().WithMessage("Title is required.")
			.MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

		RuleFor(x => x.Description)
			.MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

		RuleFor(x => x.DueDate)
			.GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
			.When(x => x.DueDate.HasValue);

		RuleFor(x => x.Priority)
			.IsInEnum().WithMessage("Invalid priority value.")
			.When(x => x.Priority.HasValue);
	}
}
