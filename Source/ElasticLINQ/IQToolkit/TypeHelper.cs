// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
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

        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        public static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        public static Type GetNullAssignableType(Type type)
        {
            return !IsNullAssignable(type) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }

        public static ConstantExpression GetNullConstant(Type type)
        {
            return Expression.Constant(null, GetNullAssignableType(type));
        }

        public static Type GetMemberType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;

            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;

            if (memberInfo is EventInfo)
                return ((EventInfo)memberInfo).EventHandlerType;

            if (memberInfo is MethodInfo)
                return memberInfo.ReflectedType;
            
            return null;
        }

        public static object GetDefault(Type type)
        {
            var isNullable = !type.IsValueType || IsNullableType(type);
            return !isNullable ? Activator.CreateInstance(type) : null;
        }

        public static bool IsReadOnly(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    var pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null;
                default:
                    return true;
            }
        }

        public static bool IsInteger(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static object Convert(object value, Type type)
        {
            // Careful, we may not want to always return the default for a type when a value is null
            if (value == null)
                return GetDefault(type);

            type = GetNonNullableType(type);
            var valueType = value.GetType();
            if (type == valueType)
                return value;

            if (type.IsEnum)
            {
                if (valueType == typeof(string))
                    return Enum.Parse(type, (string)value);

                var underlyingType = Enum.GetUnderlyingType(type);
                if (underlyingType != valueType)
                    value = System.Convert.ChangeType(value, underlyingType);

                return Enum.ToObject(type, value);
            }

            // Specifically catch a Guid to String conversion
            if (type == typeof(String) && valueType == typeof(Guid))
                return value.ToString();

            // Specifically catch a String to Guid conversion
            if (type == typeof(Guid) && valueType == typeof(String))
            {
                var stringValue = value as string;
                if (stringValue == null)
                    throw new InvalidCastException("Couldn't cast value to string.");
                return new Guid(stringValue); //Throws FormatException if the string is not a Guid
            }

            // Fallback on System.Convert
            return System.Convert.ChangeType(value, type);
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