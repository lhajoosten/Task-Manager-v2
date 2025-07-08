using FluentValidation;

namespace TaskManager.Application.Tasks.Queries.GetTasks;

public class GetTasksQueryValidator : AbstractValidator<GetTasksQuery>
{
	public GetTasksQueryValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThan(0).WithMessage("Page must be greater than 0.");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("Page size must be greater than 0.")
			.LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");

		RuleFor(x => x.Status)
			.IsInEnum().WithMessage("Invalid status value.")
			.When(x => x.Status.HasValue);

		RuleFor(x => x.Priority)
			.IsInEnum().WithMessage("Invalid priority value.")
			.When(x => x.Priority.HasValue);
	}
}
