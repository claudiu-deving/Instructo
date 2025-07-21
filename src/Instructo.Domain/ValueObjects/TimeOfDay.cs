using Domain.Shared;

namespace Domain.ValueObjects;

public record TimeOfDay(int Hour, int Minutes)
{
    public static Result<TimeOfDay> Create(string HourAndMinutes)
    {
        var errors = new List<Error>();
        if(string.IsNullOrEmpty(HourAndMinutes))
            errors.Add(new Error("Hour-Empty", "Hour cannot be empty"));
        if(errors.Count!=0)
            return Result<TimeOfDay>.WithErrors([.. errors]);

        var hourValues = HourAndMinutes.Split(':');
        if(hourValues.Length!=2)
            errors.Add(new Error("TimeOfDay-Format", "Starting Hour should contain ':'"));
        if(errors.Count!=0)
            return Result<TimeOfDay>.WithErrors([.. errors]);
        if(!int.TryParse(hourValues[0], out var hour))
            errors.Add(new Error("TimeOfDay-Format", "The hour cannot be parsed"));
        if(!int.TryParse(hourValues[1], out var minutes))
            errors.Add(new Error("TimeOfDay-Format", "The minutes cannot be parsed"));
        if(errors.Count!=0)
            return Result<TimeOfDay>.WithErrors([.. errors]);
        if(hour>24)
            errors.Add(new Error("TimeOfDay-Error", "The hour cannot be more than 24"));
        if(minutes>60)
            errors.Add(new Error("TimeOfDay-Error", "The minutes cannot be more than 60"));

        return Result<TimeOfDay>.Success(new TimeOfDay(hour, minutes));
    }

    public double ExpressedAsHour()
    {
        return Hour+(double)Minutes/60;
    }

    public override string ToString()
    {
        return $"{Hour:D2}:{Minutes:D2}";
    }
}


