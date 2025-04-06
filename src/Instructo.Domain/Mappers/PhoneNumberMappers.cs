using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.Dtos.PhoneNumbers;
using Instructo.Domain.Entities.SchoolEntities;
using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Mappers;

public static class PhoneNumberMappers
{
    public static PhoneNumberGroupDto ToDto(this PhoneNumbersGroup phoneNumbersGroup) =>
        new PhoneNumberGroupDto(phoneNumbersGroup.Name, [.. phoneNumbersGroup.PhoneNumbers.Select(x => x.ToDto())]);

    public static PhoneNumberDto ToDto(this PhoneNumber phoneNumber) =>
        new PhoneNumberDto(phoneNumber.Value, phoneNumber.Name);
}
