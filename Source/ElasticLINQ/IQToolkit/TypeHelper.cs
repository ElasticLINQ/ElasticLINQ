// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IQToolkit
{
    public static class TypeHelper
    {
        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                var foundAssignable = seqType.GetGenericArguments()
                    .Select(arg => typeof(IEnumerable<>).MakeGenericType(arg))
                    .FirstOrDefault(ienum => ienum.IsAssignableFrom(seqType));
                if (foundAssignable != null)
                    return foundAssignable;
            }

            var foundInterface = seqType.GetInterfaces().Select(FindIEnumerable).FirstOrDefault(i => i != null);
            if (foundInterface != null)
                return foundInterface;

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                return FindIEnumerable(seqType.BaseType);

            return null;
        }

        public static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            return ienum == null ? seqType : ienum.GetGenericArguments()[0];
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        public static object GetValue(this MemberInfo member, object instance)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).GetValue(instance, null);
                case MemberTypes.Field:
                    return ((FieldInfo)member).GetValue(instance);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}