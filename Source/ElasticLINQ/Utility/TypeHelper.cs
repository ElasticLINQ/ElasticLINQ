// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Various methods to make reflection and type handling a little
    /// easier.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Find the element type given a generic sequence type.
        /// </summary>
        /// <param name="sequenceType">Sequence type to examine.</param>
        /// <returns>Element type of the sequence or null if none found.</returns>
        public static Type GetSequenceElementType(Type sequenceType)
        {
            var elementType = FindIEnumerable(sequenceType);
            return elementType == null
                ? sequenceType
                : elementType.GetGenericArguments()[0];
        }

        /// <summary>
        /// Find the IEnumerable generic interface for a given sequence type.
        /// </summary>
        /// <param name="sequenceType">Sequence type to examine.</param>
        /// <returns>IEnumerable generic interface or null if not found.</returns>
        public static Type FindIEnumerable(Type sequenceType)
        {
            if (sequenceType == null || sequenceType == typeof(string))
                return null;

            if (sequenceType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(sequenceType.GetElementType());

            while (true)
            {
                foreach (var argument in sequenceType.GetGenericArguments())
                {
                    var candidateIEnumerable = typeof(IEnumerable<>).MakeGenericType(argument);
                    if (candidateIEnumerable.IsAssignableFrom(sequenceType))
                        return candidateIEnumerable;
                }

                foreach (var candidateInterface in sequenceType.GetInterfaces().Select(FindIEnumerable))
                    if (candidateInterface != null)
                        return candidateInterface;

                if (sequenceType.BaseType == null || sequenceType.BaseType == typeof(object))
                    return null;

                sequenceType = sequenceType.BaseType;
            }
        }

        public static bool IsGenericOf(this Type type, Type genericType)
        {
            return type != null && genericType != null
                && type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        /// <summary>
        /// Determine if a type is nullable either because it is a reference type or because
        /// it uses the Nullable generic container.
        /// </summary>
        /// <param name="type">Type of the value to consider.</param>
        /// <returns>True if the type supports nullability; otherwise, false.</returns>
        public static bool IsNullable(this Type type)
        {
            return !type.IsValueType || type.IsGenericOf(typeof(Nullable<>));
        }

        /// <summary>
        /// Create a default value for either a value type or reference type. 
        /// </summary>
        /// <param name="type">Type of the value to create.</param>
        /// <returns>Default value for this type.</returns>
        public static object CreateDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}