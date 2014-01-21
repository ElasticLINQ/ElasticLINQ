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
        public static Type GetSequenceElementType(Type sequenceType)
        {
            var elementType = FindIEnumerable(sequenceType);
            return elementType == null
                ? sequenceType
                : elementType.GetGenericArguments()[0];
        }

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
    }
}