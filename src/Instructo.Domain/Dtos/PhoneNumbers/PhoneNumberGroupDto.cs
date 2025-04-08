namespace Domain.Dtos.PhoneNumbers;

public readonly record struct PhoneNumberGroupDto(string Name, List<PhoneNumberDto> PhoneNumbers);
