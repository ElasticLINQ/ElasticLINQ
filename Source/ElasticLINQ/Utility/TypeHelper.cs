// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Various methods to make reflection and type handling a little
    /// easier.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Get the return type of a method, property or field.
        /// </summary>
        /// <param name="memberInfo">MemberInfo of the member to be examined.</param>
        /// <returns>Return type of that member.</returns>
        public static Type GetReturnType(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;

            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;

            var reflectedName = memberInfo.ReflectedType != null ? memberInfo.ReflectedType.FullName : "unknown";
            var declaredName = memberInfo.DeclaringType != null ? memberInfo.DeclaringType.FullName : "unknown";

            var typeName = memberInfo.ReflectedType == memberInfo.DeclaringType
                ? String.Format("'{0}'", reflectedName)
                : String.Format("'{0}' declared on '{1}'", reflectedName, declaredName);

            throw new NotSupportedException(String.Format("Member '{0}' on type {1} is of unsupported type '{2}'", memberInfo.Name, typeName, memberInfo.GetType().FullName));
        }

        /// <summary>
        /// Get the MemberInfo for a given lambda expression such as a property or method.
        /// </summary>
        /// <typeparam name="T">Type that declares the property.</typeparam>
        /// <typeparam name="TValue">Type of property to get the MemberInfo for.</typeparam>
        /// <param name="lambdaExpression">Lambda expression reference to the property.</param>
        /// <returns>MemberInfo for the given property (or method).</returns>
        /// <example>TypeHelper.GetMemberInfo((Customer c) => c.Name);</example>
        public static MemberInfo GetMemberInfo<T, TValue>(Expression<Func<T, TValue>> lambdaExpression)
        {
            switch (lambdaExpression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)lambdaExpression.Body).Member;

                default:
                    throw new NotSupportedException(String.Format("Selector node type of '{0}' not supported.", lambdaExpression.Body.NodeType));
            }
        }

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