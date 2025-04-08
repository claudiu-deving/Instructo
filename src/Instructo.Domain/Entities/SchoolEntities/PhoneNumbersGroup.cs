using Domain.ValueObjects;

namespace Domain.Entities.SchoolEntities;
public class PhoneNumbersGroup
{
    public string Name { get; set; } = string.Empty;
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
}
