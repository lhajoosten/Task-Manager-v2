using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using TaskManager.Api.Models.Tasks;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Tasks;
using TaskManager.Application.Tasks.Commands.ChangeTaskStatus;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Application.Tasks.Commands.DeleteTask;
using TaskManager.Application.Tasks.Commands.UpdateTask;
using TaskManager.Application.Tasks.Queries.GetTaskById;
using TaskManager.Application.Tasks.Queries.GetTasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all tasks for the current user with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get tasks", Description = "Retrieves tasks for the authenticated user with optional filtering")]
    [SwaggerResponse(200, "Tasks retrieved successfully", typeof(Result<PagedResult<TaskDto>>))]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Result<PagedResult<TaskDto>>>> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] TaskStatusType? status = null,
        [FromQuery] TaskPriorityType? priority = null,
        [FromQuery] string? search = null,
        [FromQuery] bool onlyOverdue = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksQuery(page, pageSize, status, priority, search, onlyOverdue);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get task by ID", Description = "Retrieves a specific task by its ID")]
    [SwaggerResponse(200, "Task retrieved successfully", typeof(Result<TaskDto>))]
    [SwaggerResponse(404, "Task not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Result<TaskDto>>> GetTaskById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error == "Task not found." ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create task", Description = "Creates a new task for the authenticated user")]
    [SwaggerResponse(201, "Task created successfully", typeof(Result<TaskDto>))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Result<TaskDto>>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = result.Value!.Id },
            result);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update task", Description = "Updates an existing task")]
    [SwaggerResponse(200, "Task updated successfully", typeof(Result<TaskDto>))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "Task not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Result<TaskDto>>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTaskCommand(
            id,
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error == "Task not found." ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Change task status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [SwaggerOperation(Summary = "Change task status", Description = "Changes the status of a task")]
    [SwaggerResponse(200, "Task status updated successfully", typeof(Result<TaskDto>))]
    [SwaggerResponse(400, "Invalid request")]
    [SwaggerResponse(404, "Task not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult<Result<TaskDto>>> ChangeTaskStatus(
        Guid id,
        [FromBody] ChangeTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new ChangeTaskStatusCommand(id, request.Status);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error == "Task not found." ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete task", Description = "Soft deletes a task")]
    [SwaggerResponse(204, "Task deleted successfully")]
    [SwaggerResponse(404, "Task not found")]
    [SwaggerResponse(401, "Unauthorized")]
    public async Task<ActionResult> DeleteTask(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTaskCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error == "Task not found." ? NotFound() : BadRequest();
        }

        return NoContent();
    }
}
