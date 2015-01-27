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
            Argument.EnsureNotNull("value", value);

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
            Argument.EnsureNotNull("value", value);

            return value.Length < 1
                ? value
                : value + (value.EndsWith("s", StringComparison.Ordinal) ? "" : "s");
        }

        /// <summary>
        /// Determine a property in the given type that is suitable for using
        /// as a discriminator to ensure only documents of this type
        /// are selected from Elasticsearch.
        /// </summary>
        /// <param name="type">Type to examine for a suitable discriminator property.</param>
        /// <returns>PropertyInfo for a suitable property to use as a discriminator.</returns>
        /// <remarks>
        /// A discriminator should be a public read/write instance property that is not generic.
        /// </remarks>
        public static PropertyInfo GetDiscriminatorProperty(Type type)
        {
            var property = type.GetTypeInfo()
                .DeclaredProperties
                .FirstOrDefault(p => p.CanRead && p.CanWrite &&
                    p.PropertyType.GetTypeInfo().IsValueType && !p.PropertyType.GetTypeInfo().IsGenericType &&
                    p.GetMethod.IsPublic && !p.GetMethod.IsStatic);

            if (property == null)
                throw new InvalidOperationException("A default query with no additional criteria (e.g. context.Query<Foo>.ToList()) must have at least one read/write non-nullable property so a correct ElasticSearch query can be generated. To resolve this, add additional criteria to your query that uses one of the type's properties (e.g. .Where(f => f.Bar != null))");

            return property;
        }
    }
}