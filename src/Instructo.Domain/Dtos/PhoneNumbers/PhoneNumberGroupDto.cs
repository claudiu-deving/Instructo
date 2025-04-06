using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.Domain.Dtos.PhoneNumbers;

public readonly record struct PhoneNumberGroupDto(string Name, List<PhoneNumberDto> PhoneNumbers);
