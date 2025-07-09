using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<Result<UserDto>>;
