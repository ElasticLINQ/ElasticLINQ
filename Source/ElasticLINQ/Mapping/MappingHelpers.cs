// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Common techniques for remapping names used between the various mappings.
    /// </summary>
    public static class MappingHelpers
    {
        public static string ToCamelCase(this string value)
        {
            if (value.Length < 2) // Don't camelcase or pluralize 1 letter
                return value.ToLower();

            return value.Substring(0, 1).ToLower() + value.Substring(1);
        }

        public static string ToPlural(this string value)
        {
            return value + (value.EndsWith("s") ? "" : "s");
        }
    }
}