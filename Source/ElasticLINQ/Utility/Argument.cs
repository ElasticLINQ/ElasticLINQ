// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;

namespace ElasticLinq.Utility
{
    /// <summary>
    /// Argument validation.
    /// </summary>
    public static class Argument
    {
        public static void EnsureNotNull(string paramName, object value)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void EnsureNotBlank(string paramName, string value)
        {
            EnsureNotNull(paramName, value);
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Cannot be a blank string.", paramName);
        }

        public static void EnsureIsAssignableFrom<T>(string paramName, Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException(string.Format("Type {0} must be assignable from {1}", type.Name, typeof(T).Name), paramName);
        }

        public static void EnsureIsDefinedEnum<T>(string paramName, T value) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentOutOfRangeException(paramName, string.Format("Must be a defined {0} enum value.", typeof(T)));
        }
    }
}