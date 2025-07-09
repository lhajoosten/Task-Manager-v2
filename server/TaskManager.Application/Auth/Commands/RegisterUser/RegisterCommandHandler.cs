using AutoMapper;
using MediatR;
using TaskManager.Application.Auth.Services;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Auth.Commands.RegisterUser;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
	private readonly IUserRepository _userRepository;
	private readonly IApplicationDbContext _context;
	private readonly IPasswordService _passwordService;
	private readonly ITokenService _tokenService;
	private readonly IMapper _mapper;

	public RegisterCommandHandler(
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

	public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		// Check if user already exists
		if (await _userRepository.ExistsAsync(request.Email, cancellationToken))
		{
			return Result.Failure<AuthResponseDto>("A user with this email already exists.");
		}

		// Hash password
		var passwordHash = _passwordService.HashPassword(request.Password);

		// Create user
		var user = new User(request.Email, request.FirstName, request.LastName, passwordHash);

		await _userRepository.AddAsync(user, cancellationToken);
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
			ExpiresAt = DateTime.UtcNow.AddHours(1) // Adjust based on your JWT expiry
		};

		return Result.Success(response);
	}
}
