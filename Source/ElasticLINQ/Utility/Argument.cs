// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Argument validation static helpers to reduce noise in other methods.
    /// </summary>
    [DebuggerStepThrough]
    static class Argument
    {
        /// <summary>
        /// Throw an ArgumentNullException if the object is null.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="value">Object to be checked.</param>
        public static void EnsureNotNull(string argumentName, object value)
        {
            if (value == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throw an ArgumentOutOfRangeException if the TimeSpan is not positive.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="value">TimeSpan to be checked.</param>
        public static void EnsurePositive(string argumentName, TimeSpan value)
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(argumentName, "Must be a positive TimeSpan.");
        }

        /// <summary>
        /// Throw an ArgumentException if the string is blank.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="value">String to be checked.</param>
        public static void EnsureNotBlank(string argumentName, string value)
        {
            EnsureNotNull(argumentName, value);
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cannot be a blank string.", argumentName);
        }

        /// <summary>
        /// Throw an ArgumentOutOfRangeException if the enum is not defined.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="value">Enum to be checked.</param>
        /// <typeparam name="TEnum">Type of the enum being checked.</typeparam>
        public static void EnsureIsDefinedEnum<TEnum>(string argumentName, TEnum value) where TEnum : struct
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentOutOfRangeException(argumentName, $"Must be a defined {typeof(TEnum)} enum value.");
        }

        /// <summary>
        /// Throw an ArgumentOutOfRangeException if the collection is empty or null.
        /// </summary>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="values">Array to be checked.</param>
        public static void EnsureNotEmpty(string argumentName, ICollection values)
        {
            if (values == null || values.Count < 1)
                throw new ArgumentOutOfRangeException(argumentName, "Must contain one or more values.");
        }
    }
}