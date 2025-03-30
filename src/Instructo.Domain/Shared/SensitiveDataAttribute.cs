using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instructo.Domain.Shared
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveDataAttribute : Attribute
    {
        public string ReplacementValue { get; }

        public SensitiveDataAttribute(string replacementValue = "[REDACTED]")
        {
            ReplacementValue=replacementValue;
        }
    }
}
