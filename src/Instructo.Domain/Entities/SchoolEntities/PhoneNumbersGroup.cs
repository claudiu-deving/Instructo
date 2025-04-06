using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Instructo.Domain.ValueObjects;

namespace Instructo.Domain.Entities.SchoolEntities;
public class PhoneNumbersGroup
{
    public string Name { get; set; } = string.Empty;
    public List<PhoneNumber> PhoneNumbers { get; set; } = [];
}
