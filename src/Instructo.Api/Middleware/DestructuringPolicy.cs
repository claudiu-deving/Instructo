using Instructo.Domain.Shared;

using Serilog.Core;
using Serilog.Events;

using System.Collections;
using System.Reflection;

namespace Instructo.Api.Middleware;

public class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        result=null;

        if(value==null)
            return false;

        var type = value.GetType();

        // Skip primitive types and strings
        if(type.IsPrimitive||type==typeof(string)||type==typeof(DateTime))
            return false;

        // Handle collection types if needed
        if(value is IEnumerable enumerable&&!(value is string))
        {
            // Handle collections differently if required
            return false;
        }

        // Process complex objects
        var properties = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);
        var dictionary = new Dictionary<ScalarValue, LogEventPropertyValue>();

        foreach(var property in properties)
        {
            var propertyValue = property.GetValue(value);

            // Check if property has SensitiveData attribute
            var sensitiveAttr = property.GetCustomAttribute<SensitiveDataAttribute>();
            var scalar = new ScalarValue(property.Name);
            if(sensitiveAttr!=null)
            {
                dictionary[scalar]=propertyValueFactory.CreatePropertyValue(sensitiveAttr.ReplacementValue);
            }
            else
            {
                // Recursively destructure non-sensitive properties
                dictionary[scalar]=propertyValueFactory.CreatePropertyValue(propertyValue, destructureObjects: true);
            }
        }

        result=new DictionaryValue(dictionary.Select(x => x));
        return true;
    }
}