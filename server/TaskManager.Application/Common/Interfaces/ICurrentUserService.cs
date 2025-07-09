using TaskManager.Domain.ValueObjects;

namespace TaskManager.Application.Common.Interfaces;

public interface ICurrentUserService
{
    UserId? GetCurrentUserId();
    bool IsAuthenticated { get; }
}
