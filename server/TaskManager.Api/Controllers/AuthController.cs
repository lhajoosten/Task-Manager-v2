using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Api.Models.Auth;
using TaskManager.Application.Auth;
using TaskManager.Application.Auth.Commands.ChangePassword;
using TaskManager.Application.Auth.Commands.LoginUser;
using TaskManager.Application.Auth.Commands.RegisterUser;
using TaskManager.Application.Auth.Queries.GetCurrentUser;
using TaskManager.Application.Common.Models;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;

	public AuthController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Register a new user account
	/// </summary>
	[HttpPost("register")]
	[SwaggerOperation(Summary = "Register user", Description = "Creates a new user account")]
	[SwaggerResponse(201, "User registered successfully", typeof(Result<AuthResponseDto>))]
	[SwaggerResponse(400, "Invalid request or user already exists")]
	public async Task<ActionResult<Result<AuthResponseDto>>> Register(
		[FromBody] Models.Auth.RegisterRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new RegisterCommand(
			request.Email,
			request.Password,
			request.ConfirmPassword,
			request.FirstName,
			request.LastName);

		var result = await _mediator.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return BadRequest(result);
		}

		return CreatedAtAction(nameof(GetCurrentUser), result);
	}

	/// <summary>
	/// Authenticate user and get access token
	/// </summary>
	[HttpPost("login")]
	[SwaggerOperation(Summary = "Login user", Description = "Authenticates user and returns access token")]
	[SwaggerResponse(200, "Login successful", typeof(Result<AuthResponseDto>))]
	[SwaggerResponse(401, "Invalid credentials")]
	public async Task<ActionResult<Result<AuthResponseDto>>> Login(
		[FromBody] Models.Auth.LoginRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new LoginCommand(request.Email, request.Password);
		var result = await _mediator.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return Unauthorized(result);
		}

		return Ok(result);
	}

	/// <summary>
	/// Get current authenticated user information
	/// </summary>
	[HttpGet("me")]
	[Authorize]
	[SwaggerOperation(Summary = "Get current user", Description = "Retrieves the current authenticated user's information")]
	[SwaggerResponse(200, "User information retrieved", typeof(Result<UserDto>))]
	[SwaggerResponse(401, "Unauthorized")]
	public async Task<ActionResult<Result<UserDto>>> GetCurrentUser(CancellationToken cancellationToken = default)
	{
		var query = new GetCurrentUserQuery();
		var result = await _mediator.Send(query, cancellationToken);

		if (result.IsFailure)
		{
			return Unauthorized(result);
		}

		return Ok(result);
	}

	/// <summary>
	/// Change current user's password
	/// </summary>
	[HttpPost("change-password")]
	[Authorize]
	[SwaggerOperation(Summary = "Change password", Description = "Changes the current user's password")]
	[SwaggerResponse(200, "Password changed successfully")]
	[SwaggerResponse(400, "Invalid request or incorrect current password")]
	[SwaggerResponse(401, "Unauthorized")]
	public async Task<ActionResult<Result>> ChangePassword(
		[FromBody] ChangePasswordRequest request,
		CancellationToken cancellationToken = default)
	{
		var command = new ChangePasswordCommand(
			request.CurrentPassword,
			request.NewPassword,
			request.ConfirmNewPassword);

		var result = await _mediator.Send(command, cancellationToken);

		if (result.IsFailure)
		{
			return BadRequest(result);
		}

		return Ok(result);
	}

	/// <summary>
	/// Logout user (client-side token removal)
	/// </summary>
	[HttpPost("logout")]
	[Authorize]
	[SwaggerOperation(Summary = "Logout user", Description = "Logs out the current user (client should remove token)")]
	[SwaggerResponse(200, "Logout successful")]
	public ActionResult Logout()
	{
		// With JWT, logout is typically handled client-side by removing the token
		// You could implement token blacklisting here if needed
		return Ok(new { message = "Logout successful" });
	}
}
