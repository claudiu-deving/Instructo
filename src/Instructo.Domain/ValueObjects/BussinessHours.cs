using System.Text.Json.Serialization;

using Domain.Dtos;
using Domain.Shared;

namespace Domain.ValueObjects;

public record BussinessHours
{
    public List<BussinessHoursEntry> BussinessHoursEntries { get; } = [];
    private BussinessHours(List<BussinessHoursEntry> bussinessHoursEntries)
    {
        BussinessHoursEntries=bussinessHoursEntries;
    }
    [JsonConstructor]
    private BussinessHours()
    {

    }
    public static BussinessHours Empty => new BussinessHours([]);

    public static Result<BussinessHours> Create(List<BusinessHoursEntryDto> bussinessHoursDto)
    {
        List<BussinessHoursEntry> entries = [];
        List<Error> errors = [];
        bussinessHoursDto.ForEach(entry =>
        {
            var daysOfTheWeek = new List<DayOfWeek>();
            try
            {
                entry.DaysOfTheWeek.ForEach(day => daysOfTheWeek.Add(ToDayOfWeekEnum(day)));
            }catch(ArgumentException ex)
            {
               errors.Add(new Error("DayOfTheWeekFormat", ex.Message));
            }
            List<TimeOfDayInterval> times = [];
            entry.Intervals.ForEach(interval =>
            {
                TimeOfDayInterval.Create(interval.StartingHourAndMinute, interval.EndingHourAndMinute)
                .OnError(errors.AddRange)
                .OnSuccess(ok => times.Add(ok));
            });
            entries.Add(new BussinessHoursEntry(daysOfTheWeek, times));
        });
        if(errors.Count!=0)
            return Result<BussinessHours>.WithErrors([.. errors]);
        return Result<BussinessHours>.Success(new BussinessHours(entries));
    }

    private static DayOfWeek ToDayOfWeekEnum(string dayOfWeek) => dayOfWeek switch
    {
        "Monday" => DayOfWeek.Monday,
        "Tuesday" => DayOfWeek.Tuesday,
        "Wednesday" => DayOfWeek.Wednesday,
        "Thursday" => DayOfWeek.Thursday,
        "Friday" => DayOfWeek.Friday,
        "Saturday" => DayOfWeek.Saturday,
        "Sunday" => DayOfWeek.Sunday,
        _ => throw new ArgumentException($"{dayOfWeek} is not a day of the week")
    };
}