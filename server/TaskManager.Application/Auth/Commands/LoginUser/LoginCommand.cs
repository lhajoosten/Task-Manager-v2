using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Auth.Commands.LoginUser;

public record LoginCommand(
	string Email,
	string Password
) : IRequest<Result<AuthResponseDto>>;
