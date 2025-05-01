using Domain.ValueObjects;

namespace Domain.Dtos;

public readonly record struct BusinessHoursEntryDto(List<string> DaysOfTheWeek, List<HourIntervals> Intervals);
