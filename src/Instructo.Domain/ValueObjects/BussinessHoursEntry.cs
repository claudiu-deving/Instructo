namespace Domain.ValueObjects;

public record BussinessHoursEntry(List<DayOfWeek> DayOfTheWeek, List<TimeOfDayInterval> Times);