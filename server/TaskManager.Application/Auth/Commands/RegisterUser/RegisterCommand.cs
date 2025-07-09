using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Auth.Commands.RegisterUser;

public record RegisterCommand(
	string Email,
	string Password,
	string ConfirmPassword,
	string FirstName,
	string LastName
) : IRequest<Result<AuthResponseDto>>;
