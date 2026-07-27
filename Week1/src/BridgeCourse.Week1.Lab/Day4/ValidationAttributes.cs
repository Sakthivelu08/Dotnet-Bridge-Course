using System;
using System.Collections.Generic;
using System.Reflection;

namespace BridgeCourse.Week1.Lab.Day4
{
    /// <summary>
    /// Custom validation attribute to restrict string maximum length.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxLengthNoAttribute : Attribute
    {
        public int MaxLength { get; }

        public MaxLengthNoAttribute(int maxLength)
        {
            if (maxLength <= 0)
            {
                throw new ArgumentException("Max length must be greater than zero.", nameof(maxLength));
            }
            MaxLength = maxLength;
        }
    }

    /// <summary>
    /// User model class containing properties annotated with the custom attribute.
    /// </summary>
    public class User
    {
        [MaxLengthNo(10)]
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Reflection-based engine that inspects objects and validates properties marked with MaxLengthNoAttribute.
    /// </summary>
    public static class ValidationEngine
    {
        /// <summary>
        /// Inspects the object for MaxLengthNo attributes and reports validation warnings.
        /// </summary>
        public static (bool IsValid, List<string> Warnings) Validate(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var warnings = new List<string>();
            Type type = obj.GetType();

            // Iterate over all public instance properties
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in properties)
            {
                // Retrieve custom MaxLengthNo attribute if present
                var attr = prop.GetCustomAttribute<MaxLengthNoAttribute>();
                if (attr != null)
                {
                    // Ensure the property is indeed a string before checking length
                    if (prop.PropertyType == typeof(string))
                    {
                        string value = (string?)prop.GetValue(obj) ?? string.Empty;
                        if (value.Length > attr.MaxLength)
                        {
                            string warning = $"Warning: Property '{prop.Name}' (value: '{value}') exceeds the maximum limit of {attr.MaxLength} characters. Current length: {value.Length}.";
                            warnings.Add(warning);
                        }
                    }
                    else
                    {
                        // Log a structural warning if applied to a non-string property
                        warnings.Add($"Structural Error: MaxLengthNoAttribute applied to non-string property '{prop.Name}'.");
                    }
                }
            }

            return (warnings.Count == 0, warnings);
        }
    }
}
