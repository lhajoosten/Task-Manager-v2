using FluentValidation;

namespace TaskManager.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
	public ChangePasswordCommandValidator()
	{
		RuleFor(x => x.CurrentPassword)
			.NotEmpty().WithMessage("Current password is required.");

		RuleFor(x => x.NewPassword)
			.NotEmpty().WithMessage("New password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
			.Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one digit.");

		RuleFor(x => x.ConfirmNewPassword)
			.NotEmpty().WithMessage("Password confirmation is required.")
			.Equal(x => x.NewPassword).WithMessage("New password and confirmation password do not match.");
	}
}
