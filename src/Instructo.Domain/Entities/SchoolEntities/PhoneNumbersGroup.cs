using Domain.Models;

namespace Domain.Entities.SchoolEntities;
public class PhoneNumbersGroup
{
    public static PhoneNumbersGroup Empty { get; set; } = new PhoneNumbersGroup();
    public string Name { get; set; } = string.Empty;
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
}
