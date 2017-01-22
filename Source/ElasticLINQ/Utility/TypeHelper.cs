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
    static class TypeHelper
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

            var declaredName = memberInfo.DeclaringType != null ? memberInfo.DeclaringType.FullName : "unknown";
            throw new NotSupportedException($"Member '{memberInfo.Name}' on type {declaredName} is of unsupported type '{memberInfo.GetType().FullName}'");
        }

        /// <summary>
        /// Get the MemberInfo for a given lambda expression to a property.
        /// </summary>
        /// <typeparam name="T">Type that declares the property.</typeparam>
        /// <typeparam name="TValue">Type of property to get the MemberInfo for.</typeparam>
        /// <param name="lambdaExpression">Lambda expression reference to the property.</param>
        /// <returns>MemberInfo for the given property.</returns>
        /// <example>TypeHelper.GetMemberInfo((Customer c) => c.Name);</example>
        public static MemberInfo GetMemberInfo<T, TValue>(Expression<Func<T, TValue>> lambdaExpression)
        {
            switch (lambdaExpression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)lambdaExpression.Body).Member;

                default:
                    throw new NotSupportedException($"Selector node type of '{lambdaExpression.Body.NodeType}' not supported.");
            }
        }

        /// <summary>
        /// Get the MethodInfo for a method on a type given a predicate to identify it.
        /// </summary>
        /// <remarks>
        /// This will throw if there are zero or more than one matches.
        /// </remarks>
        /// <param name="type">Type to examine for the MethodInfo.</param>
        /// <param name="predicate">Predicate that identifies which method to select.</param>
        /// <returns>MethodInfo belonging to the method identified.</returns>
        public static MethodInfo GetMethodInfo(this Type type, Func<MethodInfo, bool> predicate)
        {
            return type.GetTypeInfo().DeclaredMethods.Single(predicate);
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
                : elementType.GenericTypeArguments[0];
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

            var sequenceTypeInfo = sequenceType.GetTypeInfo();

            while (true)
            {
                foreach (var argument in sequenceType.GenericTypeArguments)
                {
                    var candidateIEnumerable = typeof(IEnumerable<>).MakeGenericType(argument);
                    if (candidateIEnumerable.IsAssignableFrom(sequenceType))
                        return candidateIEnumerable;
                }

                foreach (var candidateInterface in sequenceTypeInfo.ImplementedInterfaces.Select(FindIEnumerable))
                    if (candidateInterface != null)
                        return candidateInterface;

                if (sequenceTypeInfo.BaseType == null || sequenceTypeInfo.BaseType == typeof(object))
                    return null;

                sequenceTypeInfo = sequenceTypeInfo.BaseType.GetTypeInfo();
            }
        }

        /// <summary>
        /// Determine if the type implements the given generic type definition.
        /// </summary>
        /// <param name="type">Type being examined.</param>
        /// <param name="genericType">Generic type being tested for.</param>
        /// <returns>True if the type implements the generic type; false otherwise.</returns>
        public static bool IsGenericOf(this Type type, Type genericType)
        {
            return type != null && genericType != null
                && type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        /// <summary>
        /// Determine if a type is nullable either because it is a reference type or because
        /// it uses the Nullable generic container.
        /// </summary>
        /// <param name="type">Type of the value to consider.</param>
        /// <returns>True if the type supports nullability; otherwise, false.</returns>
        public static bool IsNullable(this Type type)
        {
            return !type.GetTypeInfo().IsValueType || type.IsGenericOf(typeof(Nullable<>));
        }

        /// <summary>
        /// Create a default value for either a value type or reference type. 
        /// </summary>
        /// <param name="type">Type of the value to create.</param>
        /// <returns>Default value for this type.</returns>
        public static object CreateDefault(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Determines if the target type can be assigned to the source type.
        /// </summary>
        /// <param name="source">Source type being checked to see if it is assignable.</param>
        /// <param name="target">Target type being checked to see if it is assignable.</param>
        /// <returns>True if target type can be assigned from source type.</returns>
        public static bool IsAssignableFrom(this Type source, Type target)
        {
            return (source.GetTypeInfo().IsAssignableFrom(target.GetTypeInfo()));
        }

        /// <summary>
        /// Get a method signature for identifying unsupported method overloads that does not include
        /// the return type.
        /// </summary>
        /// <param name="methodInfo">Method info to obtain simplified signature for.</param>
        /// <returns>String containing the simplified method signature.</returns>
        public static string GetSimpleSignature(this MethodInfo methodInfo)
        {
            return methodInfo.ToString().Substring(methodInfo.ReturnType.ToString().Length + 1);
        }
    }
}