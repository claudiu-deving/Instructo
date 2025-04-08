using Domain.Dtos.PhoneNumbers;
using Domain.Entities.SchoolEntities;
using Domain.ValueObjects;

namespace Domain.Mappers;

public static class PhoneNumberMappers
{
    public static PhoneNumberGroupDto ToDto(this PhoneNumbersGroup phoneNumbersGroup) =>
        new PhoneNumberGroupDto(phoneNumbersGroup.Name, [.. phoneNumbersGroup.PhoneNumbers.Select(x => x.ToDto())]);

    public static PhoneNumberDto ToDto(this PhoneNumber phoneNumber) =>
        new PhoneNumberDto(phoneNumber.Value, phoneNumber.Name);
}
