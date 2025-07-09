using AutoMapper;

using TaskManager.Application.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Mappings;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskItem, TaskDto>()
            .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedToId))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()));
    }
}
