namespace Domain.Shared
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
