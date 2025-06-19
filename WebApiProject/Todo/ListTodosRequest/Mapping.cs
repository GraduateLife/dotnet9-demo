using AutoMapper;

namespace WebApiProject.Todo.ListTodosRequest;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<ListTodosHttpRequest, ListTodosRequest>()
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted ?? false))
            .ForMember(dest => dest.SearchTerm, opt => opt.MapFrom(src => src.Search)) // Default to false if null
            .ForMember(dest => dest.OrderByField,
                opt => opt.MapFrom(src => src.F ?? "Title")) // Default to "Id" if null
            .ForMember(dest => dest.Ordering, opt => opt.MapFrom(src => src.O ?? "asc")) // Default to "asc" if null
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.P ?? 1)) // Default to 1 if null
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.Size ?? 10)); // Default to 10 if null

        CreateMap<Data.Todo, TodoListItem>();
    }
}