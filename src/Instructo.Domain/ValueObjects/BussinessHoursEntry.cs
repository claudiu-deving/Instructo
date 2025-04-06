using Instructo.Domain.Dtos;

namespace Instructo.Domain.ValueObjects;

public record BussinessHoursEntry(List<DayOfWeek> DayOfTheWeek, List<TimeOfDayInterval> Times);