using AutoMapper;

using MediatR;

using TaskManager.Application.Auth.Services;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Auth.Commands.LoginUser;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return Result.Failure<AuthResponseDto>("Invalid email or password.");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            return Result.Failure<AuthResponseDto>("Account is deactivated. Please contact support.");
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Failure<AuthResponseDto>("Invalid email or password.");
        }

        // Record login
        user.RecordLogin();
        _userRepository.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var userDto = _mapper.Map<UserDto>(user);
        var response = new AuthResponseDto
        {
            User = userDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        return Result.Success(response);
    }
}
