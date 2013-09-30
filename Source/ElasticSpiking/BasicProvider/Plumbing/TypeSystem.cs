using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSpiking.BasicProvider
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type sequenceType)
        {
            var ienumerableInterface = FindIEnumerable(sequenceType);
            return ienumerableInterface == null
                ? sequenceType
                : ienumerableInterface.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type sequenceType)
        {
            if (sequenceType == null || sequenceType == typeof(string))
                return null;

            if (sequenceType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(sequenceType.GetElementType());

            if (sequenceType.IsGenericType)
            {
                var genericInterface = sequenceType
                        .GetGenericArguments()
                        .Select(arg => typeof(IEnumerable<>).MakeGenericType(arg))
                        .FirstOrDefault(ienum => ienum.IsAssignableFrom(sequenceType));

                if (genericInterface != null)
                    return genericInterface;
            }

            var nonGenericInterface = sequenceType.GetInterfaces().Select(FindIEnumerable).FirstOrDefault();
            if (nonGenericInterface != null)
                return nonGenericInterface;

            if (sequenceType.BaseType != null && sequenceType.BaseType != typeof(object))
                return FindIEnumerable(sequenceType.BaseType);

            return null;
        }
    }
}