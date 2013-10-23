// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using ElasticLinq.Utility;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Common techniques for remapping names used between the various mappings.
    /// </summary>
    public static class MappingHelpers
    {
        public static string ToCamelCase(this string value)
        {
            Argument.EnsureNotNull("value", value);

            return value.Length < 2
                ? value.ToLower()
                : char.ToLower(value[0]) + value.Substring(1);
        }

        public static string ToPlural(this string value)
        {
            Argument.EnsureNotNull("value", value);

            return value.Length < 1
                ? value
                : value + (value.EndsWith("s") ? "" : "s");
        }
    }
}