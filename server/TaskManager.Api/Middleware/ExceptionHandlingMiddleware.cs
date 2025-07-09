using FluentValidation;
using System.Net;
using System.Text.Json;

namespace TaskManager.Api.Middleware;

public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unhandled exception occurred");
			await HandleExceptionAsync(context, ex);
		}
	}

	private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";

		var response = new
		{
			isSuccess = false,
			error = "An error occurred while processing your request."
		};

		switch (exception)
		{
			case ValidationException validationEx:
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			response = new
			{
				isSuccess = false,
				error = $"Validation failed: {validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })}"
			};
			break;

			case UnauthorizedAccessException:
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			response = new
			{
				isSuccess = false,
				error = "Unauthorized access"
			};
			break;

			case ArgumentException argEx:
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			response = new
			{
				isSuccess = false,
				error = argEx.Message
			};
			break;

			default:
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			break;
		}

		var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		});

		await context.Response.WriteAsync(jsonResponse);
	}
}