// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Common techniques for re-mapping names used between the various mappings.
    /// </summary>
    public static class MappingHelpers
    {
        /// <summary>
        /// Convert a string to camel-case.
        /// </summary>
        /// <param name="value">Input string to be camel-cased.</param>
        /// <param name="culture">CultureInfo to be used to lower-case first character.</param>
        /// <returns>String that has been converted to camel-case.</returns>
        public static string ToCamelCase(this string value, CultureInfo culture)
        {
            Argument.EnsureNotNull(nameof(value), value);

            var words = Regex.Split(value, "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])");
            return string.Concat(words.First().ToLowerInvariant(), string.Concat(words.Skip(1)));
        }

        /// <summary>
        /// Pluralize a string.
        /// </summary>
        /// <param name="value">Input string to be pluralized.</param>
        /// <param name="culture">Culture to be used in pluralization.</param>
        /// <returns>String that has been pluralized.</returns>
        /// <remarks>
        /// This is a dumb implementation that doesn't even handle English correctly.
        /// </remarks>
        public static string ToPlural(this string value, CultureInfo culture)
        {
            Argument.EnsureNotNull(nameof(value), value);

            return value.Length < 1
                ? value
                : value + (value.EndsWith("s", StringComparison.Ordinal) ? "" : "s");
        }

        /// <summary>
        /// Find a property on the given type that is suitable for identifying
        /// documents belong to this type within Elasticsearch.
        /// </summary>
        /// <param name="type">Type to examine for a suitable property.</param>
        /// <returns>PropertyInfo for a suitable property to use as a type selector or null if none are available.</returns>
        /// <remarks>
        /// A type selection property should be a public read/write instance property that is not generic.
        /// </remarks>
        public static PropertyInfo GetTypeSelectionProperty(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return typeInfo.DeclaredProperties.FirstOrDefault(BasicTypeSelectionPropertyCriteria)
                   ?? RequiredProperty(typeInfo);
        }

        /// <summary>
        /// Determine if a property is suitable to use as a type selector.
        /// </summary>
        /// <remarks>
        /// A type selection property should be a public read/write instance property that is not generic.
        /// </remarks>
        public static Func<PropertyInfo, bool> BasicTypeSelectionPropertyCriteria = p =>
            p.CanRead && p.CanWrite && p.GetMethod.IsPublic && !p.GetMethod.IsStatic &&
            p.PropertyType.GetTypeInfo().IsValueType && !p.PropertyType.GetTypeInfo().IsGenericType;

        /// <summary>
        /// Find the first property of a class that has a 'Required' attribute.
        /// </summary>
        /// <remarks>
        /// This is likely System.ComponentModel.DataAnnotations.RequiredAttribute but doesn't have to be.
        /// </remarks>
        public static Func<TypeInfo, PropertyInfo> RequiredProperty = t =>
            t.DeclaredProperties.FirstOrDefault(f => f.CustomAttributes.Any(a => a.AttributeType.Name == "RequiredAttribute"));
    }
}