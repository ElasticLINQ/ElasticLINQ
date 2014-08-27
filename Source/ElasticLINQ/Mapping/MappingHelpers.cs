// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Common techniques for re-mapping names used between the various mappings.
    /// </summary>
    public static class MappingHelpers
    {
        public static string ToCamelCase(this string value, CultureInfo culture)
        {
            Argument.EnsureNotNull("value", value);

            return value.Length < 2
                ? culture.TextInfo.ToLower(value)
                : culture.TextInfo.ToLower(value[0]) + value.Substring(1);
        }

        public static string ToPlural(this string value, CultureInfo culture)
        {
            Argument.EnsureNotNull("value", value);

            return value.Length < 1
                ? value
                : value + (value.EndsWith("s", StringComparison.Ordinal) ? "" : "s");
        }

        public static PropertyInfo GetSelectionProperty(Type type)
        {
            var property = type.GetTypeInfo()
                .DeclaredProperties
                .FirstOrDefault(p => p.CanRead && p.CanWrite &&
                    p.PropertyType.GetTypeInfo().IsValueType && !p.PropertyType.GetTypeInfo().IsGenericType &&
                    p.GetMethod.IsPublic && !p.GetMethod.IsStatic);

            if (property == null)
                throw new InvalidOperationException(String.Format("Could not find public read/write non-generic value type property to use for a default query against {0}.", type.FullName));

            return property;
        }
    }
}