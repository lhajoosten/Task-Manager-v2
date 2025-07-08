using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace TaskManager.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
	private readonly ILogger<LoggingBehaviour<TRequest>> _logger;

	public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest>> logger)
	{
		_logger = logger;
	}

	public Task Process(TRequest request, CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;
		_logger.LogInformation("TaskManager Request: {Name} {@Request}", requestName, request);
		return Task.CompletedTask;
	}
}
