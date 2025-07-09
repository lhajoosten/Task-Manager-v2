using MediatR;

using TaskManager.Application.Auth.Services;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordService _passwordService;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUserService = currentUserService;
        _passwordService = passwordService;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result.Failure("User not authenticated.");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found.");
        }

        // Verify current password
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result.Failure("Current password is incorrect.");
        }

        // Hash new password and update
        var newPasswordHash = _passwordService.HashPassword(request.NewPassword);
        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
