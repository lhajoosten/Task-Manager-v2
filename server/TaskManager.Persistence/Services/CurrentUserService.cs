using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Persistence.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserId? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim?.ToString()) || !Guid.TryParse(userIdClaim?.ToString(), out var userId))
        {
            return null;
        }

        return UserId.From(userId);
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}