using System.Text.RegularExpressions;
using AutoMapper;

namespace WebApiProject.Todo.CreateTodoCommand;

public partial class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<CreateTodoHttpRequest, CreateTodoCommand>()
            .ForMember(
                dest => dest.StartAtUtc,
                opt => opt.MapFrom(src => src.StartAtUtc ?? DateTime.UtcNow))
            .ForMember(
                dest => dest.DueAtUtc,
                opt => opt.MapFrom(src => AddDurationToDueAtUtc(src.StartAtUtc ?? DateTime.UtcNow, src.Duration))
            );

        CreateMap<CreateTodoCommand, Data.Todo>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false));
    }

    private static DateTime AddDurationToDueAtUtc(DateTime timeStart, string durationString)
    {
        var match = MatchDurationString().Match(durationString);
        if (!match.Success)
            throw new ArgumentException(
                $"Invalid duration format: '{durationString}'. Expected formats like '1h', '3d'.");

        var amount = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value.ToLower();

        return unit switch
        {
            "h" => timeStart.AddHours(amount),
            "d" => timeStart.AddDays(amount),
            "m" => timeStart.AddMinutes(amount),
            "s" => timeStart.AddSeconds(amount),
            _ => throw new ArgumentException($"Unsupported duration unit: '{unit}'.")
        };
    }

    [GeneratedRegex(@"(\d+)([hdms])", RegexOptions.IgnoreCase)]
    private static partial Regex MatchDurationString();
}