using Domain.Shared;

namespace Domain.ValueObjects;

public record TimeOfDayInterval(TimeOfDay Start, TimeOfDay End, TimeSpan Span)
{
    /// <summary>
    /// Create a TimeOfDayInterval entry, should follow the preestablished format
    /// </summary>
    /// <param name="StartingHour">HH:mm</param>
    /// <param name="EndingHour">HH:mm</param>
    /// <returns></returns>
    public static Result<TimeOfDayInterval> Create(string StartingHour, string EndingHour)
    {
        List<Error> errors = [];
        TimeOfDay? startOfTimeOfDay = null;
        TimeOfDay.Create(StartingHour)
            .OnError(errors.AddRange)
            .OnSuccess(timeofDay => startOfTimeOfDay=timeofDay);

        TimeOfDay? endOfTimeOfDay = null;
        TimeOfDay.Create(EndingHour)
            .OnError(errors.AddRange)
            .OnSuccess(timeofDay => endOfTimeOfDay=timeofDay);
        if(errors.Count!=0||startOfTimeOfDay is null||endOfTimeOfDay is null)
            return Result<TimeOfDayInterval>.WithErrors([.. errors]);

        if(startOfTimeOfDay.Hour>=endOfTimeOfDay.Hour)
            errors.Add(new Error("TimeOfDay-Error", "The starting hour cannot be after ending hour"));
        var span = TimeSpan.FromHours(endOfTimeOfDay.ExpressedAsHour()-startOfTimeOfDay.ExpressedAsHour());
        return Result<TimeOfDayInterval>.Success(
            new TimeOfDayInterval(startOfTimeOfDay, endOfTimeOfDay, span));
    }
}


