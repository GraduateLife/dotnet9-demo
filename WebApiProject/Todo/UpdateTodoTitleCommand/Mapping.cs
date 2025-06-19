using AutoMapper;

namespace WebApiProject.Todo.UpdateTodoTitleCommand;

public class TodoMappingProfile : Profile
{
    public TodoMappingProfile()
    {
        CreateMap<UpdateTodoTitleHttpRequest, UpdateTodoTitleCommand>()
            .ForMember(dest => dest.NewTitle, opt => opt.MapFrom(src => src.Title));
    }
}