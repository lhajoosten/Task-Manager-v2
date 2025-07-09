using AutoMapper;

using MediatR;

using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result.Failure<UserDto>("User not authenticated.");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return Result.Failure<UserDto>("User not found.");
        }

        var userDto = _mapper.Map<UserDto>(user);
        return Result.Success(userDto);
    }
}
