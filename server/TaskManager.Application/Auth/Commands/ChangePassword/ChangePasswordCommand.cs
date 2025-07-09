using MediatR;

using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result>;
