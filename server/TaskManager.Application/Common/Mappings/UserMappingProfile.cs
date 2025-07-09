using AutoMapper;
using TaskManager.Application.Auth;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Mappings;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserDto>()
			.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()));
	}
}
