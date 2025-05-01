using Domain.ValueObjects;

namespace Domain.Dtos;

public readonly record struct BussinessHoursEntryDto(List<string> DaysOfTheWeek, List<HourIntervals> Intervals);
