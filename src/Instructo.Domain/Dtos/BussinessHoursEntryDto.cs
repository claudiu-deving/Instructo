using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Dtos;

public readonly record struct BussinessHoursEntryDto(List<string> DaysOfTheWeek, List<HourIntervals> Intervals);
